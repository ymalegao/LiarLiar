using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject journalPanel; // The main journal UI panel
    public GameObject cluesContent; // Content object under the Clues Scroll View
    public GameObject truthsContent; // Content object under the Truths Scroll View
    public GameObject journalItemPrefab; // Prefab for each journal entry
    public Button cluesTabButton; // Button to switch to the Clues tab
    public Button truthsTabButton; // Button to switch to the Truths tab
    public Button toggleJournalButton; // Button to open/close the journal (optional)

    [Header("Journal Data")]
    private List<string> clues = new List<string>(); // List to store clues
    private List<string> truthsAndLies = new List<string>(); // List to store truths and lies

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        journalPanel.SetActive(false); // Ensure the journal is hidden initially
    }

    private void Start()
    {
        // Attach listeners to buttons
        cluesTabButton.onClick.AddListener(ShowClues);
        truthsTabButton.onClick.AddListener(ShowTruths);

        if (toggleJournalButton != null)
        {
            toggleJournalButton.onClick.AddListener(ToggleJournal);
        }
    }

    public void ToggleJournal()
    {
        journalPanel.SetActive(!journalPanel.activeSelf);
    }

    public void ShowClues()
    {
        cluesContent.SetActive(true);
        truthsContent.SetActive(false);
    }

    public void ShowTruths()
    {
        cluesContent.SetActive(false);
        truthsContent.SetActive(true);
    }

    public void AddClue(string clue)
    {
        clues.Add(clue);
        UpdateCluesUI();
    }

    public void AddTruthsAndLies(string truth1, string truth2, string lie)
    {
        truthsAndLies.Add(truth1);
        truthsAndLies.Add(truth2);
        truthsAndLies.Add(lie);

        UpdateTruthsUI();
    }

    private void UpdateCluesUI()
    {
        // Clear existing content
        foreach (Transform child in cluesContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Add new clues
        foreach (string clue in clues)
        {
            Debug.Log($"Adding clue: {clue}");  
            GameObject item = Instantiate(journalItemPrefab, cluesContent.transform);
            item.GetComponent<TMP_Text>().text = clue;
        }
    }

    private void UpdateTruthsUI()
    {
        // Clear existing content
        foreach (Transform child in truthsContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Add new truths and lies
        foreach (string entry in truthsAndLies)
        {
            GameObject item = Instantiate(journalItemPrefab, truthsContent.transform);
            TMP_Text text = item.GetComponent<TMP_Text>();
            text.text = entry;

            // Optional: Add button for marking truths/lies
            Button itemButton = item.GetComponent<Button>();
            if (itemButton != null)
            {
                itemButton.onClick.AddListener(() => HighlightEntry(item));
            }else{
                Debug.LogWarning("Button component not found on the item.");
            }
        }
    }

    public void HighlightEntry(GameObject item)
{
    if (item == null)
    {
        Debug.LogError("Cannot highlight a null item.");
        return;
    }

    var currentItemState = item.GetComponent<JournalItemState>();
    if (currentItemState != null)
    {
        currentItemState.cycleState();
    }
    else
    {
        Debug.LogWarning("JournalItemState component not found on the item.");
    }


    
}
}

// Helper class for "Two Truths and a Lie" entries
[System.Serializable]
public class JournalEntry
{
    public string Truth1;
    public string Truth2;
    public string Lie;

    public JournalEntry(string truth1, string truth2, string lie)
    {
        Truth1 = truth1;
        Truth2 = truth2;
        Lie = lie;
    }
}
