using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class JournalManager : MonoBehaviour
{
  public static JournalManager Instance { get; private set; }

  [Header("UI Elements")]
  public GameObject journalPanel; // The main journal UI panel
  public GameObject cluesContent; // Content object under the Clues Scroll View
  public GameObject truthsContent; // Content object under the Truths Scroll View
  public TextMeshProUGUI correctnessText;
  public GameObject journalItemPrefab; // Prefab for each journal entry
  public Button cluesTabButton; // Button to switch to the Clues tab
  public Button truthsTabButton; // Button to switch to the Truths tab
  public Button toggleJournalButton; // Button to open/close the journal (optional)

  [Header("Journal Data")]
  private List<string> generalClues = new List<string>(); // General clues not tied to NPCs
  private List<string> generalTruthsAndLies = new List<string>(); // General truths and lies
  private Dictionary<string, NPCData> npcDataMap = new Dictionary<string, NPCData>(); // NPC-specific data
  private List<JournalItemState> journalEntries = new List<JournalItemState>();

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

  // Register NPCs and their associated data
  public void RegisterNPC(string npc, string name, string role, Sprite icon)
  {
    if (!npcDataMap.ContainsKey(npc))
    {
      npcDataMap.Add(npc, new NPCData(name, role, icon));
    }
    else
    {
      Debug.LogWarning($"NPC {npc} has already been registered.");
    }
  }

  // Add a general clue (not tied to an NPC)
  public void AddClue(string clue)
  {
    generalClues.Add(clue);
    UpdateGeneralCluesUI();
  }

  // Add a clue provided by an NPC
  public void AddClueFromNPC(string npc, string clue)
  {
    if (npcDataMap.ContainsKey(npc))
    {
      NPCData data = npcDataMap[npc];

      if (!data.HasBeenInteractedWith)
      {
        data.CluesGiven.Add(clue);

        // Update the journal UI for this NPC
        UpdateCluesUI(data.Name, clue, data.Icon);
      }
    }
  }

  // Add general truths and lies
  public void AddTruthsAndLies(string truth1, string truth2, string lie)
  {
    generalTruthsAndLies.Add(truth1);
    generalTruthsAndLies.Add(truth2);
    generalTruthsAndLies.Add(lie);

    UpdateGeneralTruthsUI();
  }

  // Add truths and lies provided by an NPC
  public void AddTruthsAndLiesFromNPC(string npc, List<(string dialogue, bool isTruth)> statements)
  {
    if (npcDataMap.ContainsKey(npc))
    {
      NPCData data = npcDataMap[npc];

      if (!data.HasBeenInteractedWith)
      {
        data.HasBeenInteractedWith = true;

        // Add NPC name and their Truths and Lies
        AddNPCNameAndStatements(data.Name, statements, data.Icon);
      }
      else
      {
        Debug.LogWarning($"NPC {npc} has already been interacted with.");
      }
    }
  }

  // Add the NPC's name and their three statements
  private void AddNPCNameAndStatements(string npcName, List<(string dialogue, bool isTruth)> statements, Sprite icon)
  {
    // Add the NPC name as a header
    GameObject nameItem = Instantiate(journalItemPrefab, truthsContent.transform);
    TMP_Text nameText = nameItem.GetComponent<TMP_Text>();
    nameText.text = npcName; 

    // Add icon for NPC (optional)
    Image iconImage = nameItem.transform.Find("Icon")?.GetComponent<Image>();
    if (iconImage != null && icon != null)
    {
      iconImage.sprite = icon;
    }

    // Add the three statements below the name
    foreach (var (dialogue, isTruth) in statements)
    {
      string formattedText = dialogue;
      AddStatement(formattedText, isTruth);
    }
  }

  // Add a single statement to the journal
   private void AddStatement(string statement, bool isTruth)
    {
        GameObject item = Instantiate(journalItemPrefab, truthsContent.transform);
        JournalItemState journalItem = item.GetComponent<JournalItemState>();
        journalItem.SetText(statement, isTruth);
        journalEntries.Add(journalItem);
        journalItem.OnStateChanged += UpdateCorrectCount;
    }

  private void UpdateGeneralCluesUI()
  {
    foreach (Transform child in cluesContent.transform)
    {
      Destroy(child.gameObject);
    }

    foreach (string clue in generalClues)
    {
      GameObject item = Instantiate(journalItemPrefab, cluesContent.transform);
      item.GetComponent<TMP_Text>().text = clue;
    }
  }

  private void UpdateGeneralTruthsUI()
  {
    foreach (Transform child in truthsContent.transform)
    {
      Destroy(child.gameObject);
    }

    foreach (string entry in generalTruthsAndLies)
    {
      GameObject item = Instantiate(journalItemPrefab, truthsContent.transform);
      item.GetComponent<TMP_Text>().text = entry;
    }
  }

  private void UpdateCluesUI(string npcName, string clue, Sprite icon)
  {
    GameObject item = Instantiate(journalItemPrefab, cluesContent.transform);
    TMP_Text text = item.GetComponent<TMP_Text>();
    text.text = $"{npcName}: {clue}";

    Image iconImage = item.transform.Find("Icon")?.GetComponent<Image>();
    if (iconImage != null && icon != null)
    {
      iconImage.sprite = icon;
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
  public void UpdateCorrectCount()
    {
        int correctCount = 0;
        foreach (var entry in journalEntries)
        {
            if (entry.IsMarkedCorrectly())
                correctCount++;
        }
        correctnessText.text = $"Correctly Marked: {correctCount} / 24";
    }


}
