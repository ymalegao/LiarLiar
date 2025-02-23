using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
  public static DialogueManager Instance { get; private set; }

  public Dictionary<string, List<(string dialogue, bool isTruth)>> npcStatements = new Dictionary<string, List<(string, bool)>>();

  public GameObject dialoguePanel; // Assign a panel for dialogue in the UI
  public Text dialogueText; // Assign a Text component to show dialogue

  private Queue<string> dialogueQueue = new Queue<string>();
  public bool IsDialogueActive { get; private set; } = false;

  private string currentNPCName; // Name of the NPC currently speaking

  // Event triggered when dialogue ends
  public event System.Action OnDialogueEnd;

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

  // Start the dialogue and pass the NPC's name
  public void StartDialogue(string npcName, string[] dialogueLines)
  {
    currentNPCName = npcName; // Set the current NPC name

    dialogueQueue.Clear();

    foreach (string line in dialogueLines)
    {
      dialogueQueue.Enqueue(line);
    }

    dialoguePanel.SetActive(true);
    IsDialogueActive = true;

    DisplayNextLine();
  }

  public void DisplayNextLine()
  {
    if (dialogueQueue.Count == 0)
    {
      EndDialogue();
      return;
    }

    string line = dialogueQueue.Dequeue();

    // Check for special markers and update the journal if needed
    line = CheckForClueOrTruth(line);

    StopAllCoroutines();
    StartCoroutine(TypeLine(line));
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

    // Trigger the end-of-dialogue event
    OnDialogueEnd?.Invoke();
    JournalManager.Instance.AddTruthsAndLiesFromNPC(currentNPCName, npcStatements[currentNPCName]);

  }
  private void AddToDict(string npcName, string dialogue, bool isTruth)
  {
    if (!npcStatements.ContainsKey(npcName))
    {
      npcStatements[npcName] = new List<(string, bool)>();
    }

    npcStatements[npcName].Add((dialogue, isTruth)); // Add truth/lie to the list
  }

  

  // Check if the line contains a [Clue] or [T/L] marker and update the journal
  private string CheckForClueOrTruth(string line)
  {
    if (line.StartsWith("[Clue]"))
    {
      string clue = line.Substring(6);
      JournalManager.Instance.AddClueFromNPC(currentNPCName, clue);
      return clue;
    }
    else if (line.StartsWith("[T]"))
    {
      string truth = line.Substring(4);
      AddToDict(currentNPCName, truth, true);
      return $"{truth}";
    }
    else if (line.StartsWith("[L]"))
    {
      string lie = line.Substring(4);
      AddToDict(currentNPCName, lie, false);
      return $"{lie}";
    }

    return line; // Return the unmodified line if no markers are found
  }
}
