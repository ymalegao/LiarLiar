using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
  [SerializeField] private TMP_InputField lobbyCodeInput; // Use TMP_InputField instead of InputField
  [SerializeField] private TMP_Text lobbyInfoText; // Use TMP_Text instead of Text
  [SerializeField] private TMP_Text characterInfoText; // Use TMP_Text instead of Text


  public GameObject lobbyTrigger;
  private LobbyManager lobbyManager;
  [SerializeField] private Button startGameButton;
      public TextMeshProUGUI lobbyCodeText;


      public Button copyButton;


  // [SerializeField] private Button listLobbiesButton;

  //Character selection UI
  [SerializeField] private TMP_Dropdown characterDropdown;
  [SerializeField] private GameObject characterSelectionPanel;
  [SerializeField] private Image characterPreview;
  [SerializeField] private Sprite[] characterSprites;





  private void Start()
  {
    lobbyManager = FindObjectOfType<LobbyManager>();

    if (lobbyManager == null)
    {
      Debug.LogError("LobbyManager not found in the scene!");
      return;
    }

    startGameButton.gameObject.SetActive(false);
    
    
    copyButton.onClick.AddListener(CopyLobbyCodeToClipboard);
    copyButton.gameObject.SetActive(false);



    // Subscribe to lobby creation event
    LobbyManager.OnLobbyCreated += EnableStartButton;
    LobbyManager.OnLobbyJoined += ShowCharacterSelectionPanel;
    LobbyManager.OnLobbyCreated += EnableCopyButton;


    characterDropdown.onValueChanged.AddListener(UpdateCharacterPreview);



    // If the player is the host and the lobby is already created, enable the button
    if (lobbyManager._isHost)
    {
      EnableStartButton();
    }
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (lobbyTrigger.activeSelf)
      {
        lobbyTrigger.SetActive(false);
      }
      else
      {
        lobbyTrigger.SetActive(true);
      }
    }
  }

  private void EnableStartButton()
  {
    if (startGameButton.gameObject.activeSelf) return; // Prevent duplicate activation

    startGameButton.gameObject.SetActive(true);
    startGameButton.onClick.RemoveAllListeners(); // Remove previous listeners to prevent stacking
    startGameButton.onClick.AddListener(() => lobbyManager.StartGame());
  }

  private void ShowCharacterSelectionPanel()
  {
    characterSelectionPanel.SetActive(true);
    UpdateCharacterPreview(characterDropdown.value);

  }

  private void EnableCopyButton()
    {
        copyButton.gameObject.SetActive(true);
    }

  private void UpdateCharacterPreview(int index)
  {
    characterPreview.sprite = characterSprites[index];
  }

  public async void OnCharacterSelectButtonClicked()
  {
    string selectedCharacter = characterDropdown.options[characterDropdown.value].text;
    ServerManager.Instance.setCharacterIndex(characterDropdown.value);
    characterInfoText.text = selectedCharacter;
  }

  private void OnDestroy()
  {
    // Unsubscribe from the event to avoid memory leaks
    LobbyManager.OnLobbyCreated -= EnableStartButton;
    LobbyManager.OnLobbyJoined -= ShowCharacterSelectionPanel;
    LobbyManager.OnLobbyCreated -= EnableCopyButton; // Unsubscribe to prevent memory leaks


  }

  public void CopyLobbyCodeToClipboard()
    {
        if (LobbyManager.Instance == null)
        {
            Debug.LogError("🚨 LobbyManager.Instance is NULL! Make sure LobbyManager is in the scene.");
            return;
        }

        string lobbyCode = LobbyManager.Instance.GetLobbyCode();
        if (!string.IsNullOrEmpty(lobbyCode))
        {
            GUIUtility.systemCopyBuffer = lobbyCode;
            Debug.Log($"Copied Lobby Code: {lobbyCode} ✅");
        }
        else
        {
            Debug.LogError("🚨 Lobby code is empty or not set yet!");
        }
    }

  public void OnCreateLobbyButtonClicked()
  {
    string selectedCharacter = characterDropdown.options[characterDropdown.value].text;
    _ = CreateLobbyAsync();
  }

  private async Task CreateLobbyAsync()
  {
    if (lobbyManager == null) return;

    await lobbyManager.CreateLobby("MyLobby", 4, false);
    lobbyInfoText.text = "Lobby created! Code: " + lobbyManager.GetLobbyCode();
  }

  public void OnJoinLobbyButtonClicked()
  {
    _ = JoinLobbyAsync();
  }

  private async Task JoinLobbyAsync()
  {
    if (lobbyManager == null) return;

    string lobbyCode = lobbyCodeInput.text;
    if (string.IsNullOrEmpty(lobbyCode))
    {
      lobbyInfoText.text = "Please enter a lobby code.";
      return;
    }

    await lobbyManager.JoinLobbyByCode(lobbyCode);
    lobbyInfoText.text = "Joined lobby with code: " + lobbyCode;
  }

  public void OnListLobbiesButtonClicked()
  {
    ListLobbiesAsync();
  }

  private void ListLobbiesAsync()
  {
    if (lobbyManager == null) return;

    lobbyManager.ListLobbies();
  }
}