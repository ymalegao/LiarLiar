using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public Button nextLineButton;

    public Dictionary<string, List<(string dialogueKey, bool isTruth)>> npcStatements = new Dictionary<string, List<(string, bool)>>();

    public GameObject dialoguePanel; // Assign a panel for dialogue in the UI
    public Text dialogueText; // Assign a Text component to show dialogue

    private Queue<string> dialogueQueue = new Queue<string>(); // Store localization KEYS instead of text
    public bool IsDialogueActive { get; private set; } = false;

    private string currentNPCName; // Name of the NPC currently speaking

    public event System.Action OnDialogueEnd; // Event triggered when dialogue ends

    private void Awake()
    {
        dialoguePanel.SetActive(false);
        IsDialogueActive = false;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start the dialogue and store localization keys instead of raw text
    public void StartDialogue(string npcName, string[] dialogueKeys)
    {
        currentNPCName = npcName; // Set the current NPC name
        dialogueQueue.Clear();

        foreach (string key in dialogueKeys)
        {
            dialogueQueue.Enqueue(key); // Store localization keys
        }

        dialoguePanel.SetActive(true);
        IsDialogueActive = true;

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (!NetworkManager.Singleton.IsClient) return;

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string dialogueKey = dialogueQueue.Dequeue(); // Get the key instead of raw text
        StartCoroutine(LoadLocalizedText(dialogueKey)); // Fetch translated text
    }

    private IEnumerator LoadLocalizedText(string dialogueKey)
{
    var localizedString = new LocalizedString("AveryTable", dialogueKey);
    var asyncOperation = localizedString.GetLocalizedStringAsync();

    yield return asyncOperation;

    string localizedText = asyncOperation.Result;
    Debug.Log($"Localized Text for {dialogueKey}: {localizedText}");

    // Process and update npcStatements with the actual localized text
    string processedText = CheckForClueOrTruth(dialogueKey, localizedText); 

    Debug.Log($"Localized Text after processing: {processedText}");

    StopAllCoroutines();
    StartCoroutine(TypeLine(processedText));
}


    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; // Wait for a frame between each letter
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialogueActive = false;
        dialogueText.text = ""; // Clear the dialogue text

        OnDialogueEnd?.Invoke();
        if (npcStatements.ContainsKey(currentNPCName))
        {
            JournalManager.Instance.AddTruthsAndLiesFromNPC(currentNPCName, npcStatements[currentNPCName]);
            JournalManager.Instance.ShowNPCDetails(currentNPCName);
        }
    }

    private void AddToDict(string npcName, string dialogueKey, bool isTruth)
    {
        if (!npcStatements.ContainsKey(npcName))
        {
            npcStatements[npcName] = new List<(string, bool)>();
        }

        npcStatements[npcName].Add((dialogueKey, isTruth)); // Store localization key instead of raw text
    }

    private string CheckForClueOrTruth(string dialogueKey, string line)
{
    line = line.Trim(); // Ensure no leading/trailing spaces

    if (!npcStatements.ContainsKey(currentNPCName))
    {
        npcStatements[currentNPCName] = new List<(string, bool)>();
    }

    if (line.StartsWith("[Clue]"))
    {
        string clue = line.Substring(6).Trim();
        JournalManager.Instance.AddClueFromNPC(currentNPCName, clue);
        return clue;
    }
    else if (line.StartsWith("[T]"))
    {
        Debug.Log("Truth found");
        string truth = line.Substring(4).Trim();
        npcStatements[currentNPCName].Add((truth, true)); // Store as truth
        return truth;
    }
    else if (line.StartsWith("[L]"))
    {
        Debug.Log("Lie found");
        string lie = line.Substring(4).Trim();
        npcStatements[currentNPCName].Add((lie, false)); // Store as lie
        return lie;
    }

    // If no markers, store as a neutral statement (assumed truth by default)
    npcStatements[currentNPCName].Add((line, true));
    return line;
}

}
