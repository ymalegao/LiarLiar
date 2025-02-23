using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject journalPanel;
    public Transform npcListContainer; // Left side list of NPC names

    public Transform npcStatementContainer; // Right side list of NPC statements
    public GameObject npcButtonPrefab; // Prefab for NPC name button
    public Image npcPortrait;
    public TextMeshProUGUI npcNameText;
    public Transform truthsContent;
    public Transform cluesContent; // ✅ Separate content for clues
    public GameObject journalItemPrefab;
    public Button toggleJournalButton;
    public TextMeshProUGUI correctnessText;
    public Sprite cluesIcon;


    [Header("Final Clue System")]
    public GameObject[] finalCluePrefabs;
    public Transform[] clueSpawnLocations;
    private bool finalCluesSpawned = false;


    private Dictionary<string, JournalNPCData> npcDataMap = new Dictionary<string, JournalNPCData>();
    private List<JournalItemState> journalEntries = new List<JournalItemState>();
    private List<string> generalClues = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        journalPanel.SetActive(false);

    }

    private void Start()
    {
        toggleJournalButton?.onClick.AddListener(ToggleJournal);
        //register Clues as NPC, so they are at the top of the list

        RegisterNPC("clues", "Clues", cluesIcon);
    }

    public void ToggleJournal()
    {
        journalPanel.SetActive(!journalPanel.activeSelf);
    }

    public void Update(){
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }
    }

    public void RegisterNPC(string npcId, string name, Sprite icon)
    {
        if (!npcDataMap.ContainsKey(npcId))
        {
            npcDataMap.Add(npcId, new JournalNPCData(name, icon));
            AppendNPCToList(npcId, name);
        }
    }

    private void AppendNPCToList(string npcId, string npcName)
{
    if (npcListContainer == null)
    {
        Debug.LogError("npcListContainer is not assigned in the Inspector.");
        return;
    }

    if (npcButtonPrefab == null)
    {
        Debug.LogError("npcButtonPrefab is not assigned in the Inspector.");
        return;
    }

    // Check if the NPC is already in the list
    foreach (Transform child in npcListContainer)
    {
        var textComponent = child.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null && textComponent.text == npcName)
        {
            return; // NPC already in the list
        }
    }

    // Instantiate the NPC button prefab
    GameObject buttonObj = Instantiate(npcButtonPrefab, npcListContainer);

    // Ensure the instantiated object has the correct scale and position
    buttonObj.transform.localScale = Vector3.one;
    buttonObj.transform.localPosition = Vector3.zero;

    // Set the NPC name on the button's text component
    var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
    if (buttonText == null)
    {
        Debug.LogError("The instantiated NPC button prefab is missing a TextMeshProUGUI component.");
        return;
    }
    buttonText.text = npcName;

    // Add a listener to handle button clicks
    var button = buttonObj.GetComponent<Button>();
    if (button == null)
    {
        Debug.LogError("The instantiated NPC button prefab is missing a Button component.");
        return;
    }
    button.onClick.AddListener(() => ShowNPCDetails(npcId));
}


    public void ShowNPCDetails(string npcId)
{
    if (npcDataMap.TryGetValue(npcId, out JournalNPCData npc))
    {
        Debug.Log($"Showing details for NPC: {npc.Name}");

        // Update NPC portrait
        if (npcPortrait != null && npc.Icon != null)
        {
            npcPortrait.sprite = npc.Icon;
            npcPortrait.enabled = true; // Ensure the Image component is enabled
        }
        else
        {
            Debug.LogWarning($"NPC {npc.Name} does not have an assigned portrait.");
        }

        // Update NPC name text
        if (npcNameText != null)
        {
            npcNameText.text = npc.Name;
        }

        // Clear existing statements
        foreach (Transform child in npcStatementContainer)
        {
            Destroy(child.gameObject);
        }

        // Add new statements
        foreach (var (statement, isTruth) in npc.TruthsAndLies)
        {
            GameObject entry = Instantiate(journalItemPrefab, npcStatementContainer);
            JournalItemState journalItem = entry.GetComponent<JournalItemState>();
            journalItem.SetText(statement, isTruth);
            if (npc.StatementStates.TryGetValue(statement, out var savedState))
            {
                journalItem.SetState(savedState);
            }
            journalEntries.Add(journalItem);
            journalItem.OnStateChanged += () => UpdateStatementState(npcId, statement, journalItem.GetState());


            Debug.Log($"Statement: {statement}, Is Truth: {isTruth}");
        }
    }
    else
    {
        Debug.LogError($"NPC {npcId} not found in the journal.");
    }
}

private void UpdateStatementState(string npcId, string statement, JournalItemState.State newState)
{
    if (npcDataMap.TryGetValue(npcId, out JournalNPCData npc))
    {
        npc.StatementStates[statement] = newState;
    }
}


    // public void AddClue(string clue)
    // {
    //     generalClues.Add(clue);
    //     UpdateGeneralCluesUI();
    // }

    private void UpdateGeneralCluesUI()
    {
        foreach (string clue in generalClues)
        {
            GameObject item = Instantiate(journalItemPrefab, cluesContent);
            item.GetComponent<TMP_Text>().text = clue;
        }
    }

    public void AddClue(string clue)
{
    if (npcDataMap.TryGetValue("clues", out JournalNPCData cluesData))
    {
        cluesData.TruthsAndLies.Add((clue, true)); // Assuming all clues are truths
        
            UpdateCluesUI();
        
    }
    else
    {
        Debug.LogError("Clues entry not found in the journal.");
    }
}

  private void UpdateCluesUI(){
    foreach (Transform child in npcStatementContainer)
    {
        Destroy(child.gameObject);
    }

    if (npcDataMap.TryGetValue("clues", out JournalNPCData cluesData))
    {
        foreach (var (statement, isTruth) in cluesData.TruthsAndLies)
        {
            GameObject entry = Instantiate(journalItemPrefab, npcStatementContainer);
            JournalItemState journalItem = entry.GetComponent<JournalItemState>();
            journalItem.SetText(statement, isTruth);
            journalEntries.Add(journalItem);
            journalItem.OnStateChanged += () => UpdateStatementState("clues", statement, journalItem.GetState());
        }
    }
    else
    {
        Debug.LogError("Clues entry not found in the journal.");
    }
  }

    public void AddTruthsAndLiesFromNPC(string npcId, List<(string dialogue, bool isTruth)> statements)
    {
        if (!npcDataMap.ContainsKey(npcId))
        {
            Debug.LogError($"NPC {npcId} is not registered in the journal!");
            return;
        }

        JournalNPCData npcData = npcDataMap[npcId];

        if (npcData.TruthsAndLies.Count == 0)
        {
            npcData.TruthsAndLies.AddRange(statements);
            ShowNPCDetails(npcId);
        }
        else
        {
            Debug.LogWarning($"NPC {npcId} statements already added.");
        }
    }

    public void AddClueFromNPC(string npcId, string clue)
    {
        if (!npcDataMap.ContainsKey(npcId))
        {
            Debug.LogError($"NPC {npcId} is not registered in the journal!");
            return;
        }

        JournalNPCData npcData = npcDataMap[npcId];

        if (!npcData.HasBeenInteractedWith)
        {
            npcData.HasBeenInteractedWith = true;
            npcData.CluesGiven.Add(clue);
            UpdateGeneralCluesUI();
        }
        else
        {
            Debug.LogWarning($"NPC {npcId} has already provided clues.");
        }
    }

    public void UpdateCorrectCount()
    {
        int correctCount = 0;
        foreach (var entry in journalEntries)
        {
            if (entry.IsMarkedCorrectly()) correctCount++;
        }

        correctnessText.text = $"Correctly Marked: {correctCount} / 24";
        if (correctCount >= 3 && !finalCluesSpawned)
        {
            SpawnFinalClues();
        }
    }




    private void SpawnFinalClues()
    {
        if (finalCluePrefabs.Length != clueSpawnLocations.Length)
        {
            Debug.LogError("Mismatch between final clue prefabs and spawn locations.");
            return;
        }

        for (int i = 0; i < finalCluePrefabs.Length; i++)
        {
            if (finalCluePrefabs[i] != null && clueSpawnLocations[i] != null)
            {
                Instantiate(finalCluePrefabs[i], clueSpawnLocations[i].position, Quaternion.identity);
            }
        }
        finalCluesSpawned = true;
    }
}

[System.Serializable]
public class JournalNPCData
{
    public string Name;
    public Sprite Icon;
    public bool HasBeenInteractedWith;
    public List<(string statement, bool isTruth)> TruthsAndLies;
    public List<string> CluesGiven; // ✅ Stores NPC-specific clues
    public Dictionary<string, JournalItemState.State> StatementStates;


    public JournalNPCData(string name, Sprite icon)
    {
        Name = name;
        Icon = icon;
        HasBeenInteractedWith = false;
        TruthsAndLies = new List<(string, bool)>();
        CluesGiven = new List<string>();
        StatementStates = new Dictionary<string, JournalItemState.State>();

    }
}
