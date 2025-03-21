using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance { get; private set; }

    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    [Header("UI Elements")]
    public GameObject journalPanel;
    public GameObject helper; // Assign the helper GameObject in the inspector

    private int currentNPCIndex = 0;


    private TextMeshProUGUI currentSelectedNPCText = null; // Store the currently selected NPC's text
    public Transform npcListContainer; // Left side list of NPC names

    public Transform npcStatementContainer; // Right side list of NPC statements

    public Transform npcClueContainer;
    public GameObject npcButtonPrefab; // Prefab for NPC name button
    public Image npcPortrait;
    public TextMeshProUGUI npcNameText;
    public Transform truthsContent;
    public Transform cluesContent; // ✅ Separate content for clues
    public GameObject journalItemPrefab;
    public Button toggleJournalButton;
    public TextMeshProUGUI correctnessText;

    public Image scrollBar;
    public GameObject scrollBarHandle;

    

    public int indexCount = 0;
    public Sprite cluesIcon;


    [Header("Final Clue System")]
    public GameObject[] finalCluePrefabs;
    public GameObject finalClueAlert;

    public Transform[] clueSpawnLocations;
    private bool finalCluesSpawned = false;

    [Header("UICheck")]
    [SerializeField] private GameObject selectionUI;


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
        if (finalClueAlert != null)
        {
            finalClueAlert.SetActive(false);
        }
        RegisterNPC("clues", "Clues", cluesIcon);
    }

    public void ToggleJournal()
    {
        bool isOpening = !journalPanel.activeSelf;
        journalPanel.SetActive(isOpening);
        if (audioSource != null)
        {
            audioSource.PlayOneShot(isOpening ? openSound : closeSound);
        }
    }


    public void Update(){

        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("J pressed");

            if (helper.activeSelf)
            {
                helper.SetActive(false); // Deactivate the helper
                ToggleJournal(); // Open the journal
            }

            else
            {
                ToggleJournal(); // Close the journal
            }
        }
    }

    public void RegisterNPC(string npcId, string name, Sprite icon)
    {
        if (!npcDataMap.ContainsKey(npcId))
        {
            npcDataMap.Add(npcId, new JournalNPCData(name, icon  , indexCount));
            indexCount++;
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

    //if NPC is clues, we wnt to disable the innocent and guilty buttons
    if (npcId == "clues")
    {
        buttonObj.transform.GetChild(1).gameObject.SetActive(false);
        buttonObj.transform.GetChild(2).gameObject.SetActive(false);
    }

    // Add a listener to handle button clicks
    var button = buttonObj.GetComponent<Button>();
    if (button == null)
    {
        Debug.LogError("The instantiated NPC button prefab is missing a Button component.");
        return;
    }
    button.onClick.AddListener(() => {
            ShowNPCDetails(npcId);
            HighlightSelectedNPC(buttonObj);

    });

    
}

private void HighlightSelectedNPC(GameObject selectedButton){
    if (currentSelectedNPCText != null)
    {
        currentSelectedNPCText.color = Color.black;
    }
    TextMeshProUGUI buttonText = selectedButton.GetComponentInChildren<TextMeshProUGUI>();
    if (buttonText != null)
    {
        buttonText.color = Color.yellow; // Change selected NPC text color
        currentSelectedNPCText = buttonText; // Store reference to reset later
    }
}


    public void ShowNPCDetails(string npcId)
{
    if (npcDataMap.TryGetValue(npcId, out JournalNPCData npc))
    {
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

        Debug.Log($"Showing details for NPC {npcId}");
        //if the NPC is clues, we want to set the spacing to 0 for the NPC statement container

        if (npcId == "clues")
        {
            //set npcStaementcontainer to deactivate and activate the clues container
            npcStatementContainer.gameObject.SetActive(false);
            npcClueContainer.gameObject.SetActive(true);
            showScrollbar();

            // npcStatementContainer.GetComponent<VerticalLayoutGroup>().spacing = 0;

        }
        else
        {
            npcStatementContainer.gameObject.SetActive(true);
            npcClueContainer.gameObject.SetActive(false);
            hideScrollbar();
            // npcStatementContainer.GetComponent<VerticalLayoutGroup>().spacing = -300;
        }

        // Clear existing statements
        foreach (Transform child in npcStatementContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in npcClueContainer)
        {
            Destroy(child.gameObject);
        }

        // Add new statements
        foreach (var (statement, isTruth) in npc.TruthsAndLies)
        {
            //instanciate in the npcStatementContainer if the npc is not clues else instanciate in the npcClueContainer

            if (npcId == "clues")
            {
                GameObject entry = Instantiate(journalItemPrefab, npcClueContainer);
                JournalItemState journalItem = entry.GetComponent<JournalItemState>();
                journalItem.SetText(statement, isTruth);
                journalEntries.Add(journalItem);
                journalItem.OnStateChanged += () => UpdateStatementState(npcId, statement, journalItem.GetState());
            }
            else
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
            }

            // GameObject entry = Instantiate(journalItemPrefab, npcStatementContainer);
            // JournalItemState journalItem = entry.GetComponent<JournalItemState>();
            // journalItem.SetText(statement, isTruth);
            // if (npc.StatementStates.TryGetValue(statement, out var savedState))
            // {
            //     journalItem.SetState(savedState);
            // }
            // journalEntries.Add(journalItem);
            // journalItem.OnStateChanged += () => UpdateStatementState(npcId, statement, journalItem.GetState());
        }

        currentNPCIndex = npc.index;
        
        HighlightSelectedNPC(npcListContainer.GetChild(currentNPCIndex).gameObject);
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

  public void SelectNextNPC(int direction)
{
    if (npcListContainer.childCount == 0) return;
    

    currentNPCIndex = (currentNPCIndex + direction + npcListContainer.childCount) % npcListContainer.childCount;
    Debug.Log($"Selected NPC index: {currentNPCIndex}");
    Transform selectedNPC = npcListContainer.GetChild(currentNPCIndex);
    selectedNPC.GetComponent<Button>().onClick.Invoke(); // Simulate click
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
        if (correctCount >= 20 && !finalCluesSpawned)
        {
            SpawnFinalClues();
        }
    }

    private void hideScrollbar(){
        //set Image to deactivate, not the gameobject

        scrollBar.enabled = false;
        scrollBarHandle.SetActive(false);
        
    }

    private void showScrollbar(){
        scrollBar.enabled = true;
        scrollBarHandle.SetActive(true);
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
        //temp solution - start a dialogue of one line 
        string[] ClueAlertdialogue = new string[1];
        ClueAlertdialogue[0] = "Journal1";
        DialogueManager.Instance.StartDialogue("Journal" , ClueAlertdialogue);
        // finalClueAlert.SetActive(true);
        finalCluesSpawned = true;
    }

    public void hideAlert(){
        finalClueAlert.SetActive(false);
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

    public int index;


    public JournalNPCData(string name, Sprite icon, int journalIndex)
    {
        Name = name;
        Icon = icon;
        HasBeenInteractedWith = false;
        TruthsAndLies = new List<(string, bool)>();
        CluesGiven = new List<string>();
        StatementStates = new Dictionary<string, JournalItemState.State>();
        index = journalIndex;

    }
}
