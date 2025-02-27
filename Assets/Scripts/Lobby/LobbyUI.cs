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

    // Subscribe to lobby creation event
    LobbyManager.OnLobbyCreated += EnableStartButton;
    LobbyManager.OnLobbyJoined += ShowCharacterSelectionPanel;

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
    Debug.Log("Start button enabled!");

    startGameButton.gameObject.SetActive(true);
    startGameButton.onClick.RemoveAllListeners(); // Remove previous listeners to prevent stacking
    startGameButton.onClick.AddListener(() => lobbyManager.StartGame());
  }

  private void ShowCharacterSelectionPanel()
  {
    characterSelectionPanel.SetActive(true);
    UpdateCharacterPreview(characterDropdown.value);

  }

  private void UpdateCharacterPreview(int index)
  {
    characterPreview.sprite = characterSprites[index];
  }

  public async void OnCharacterSelectButtonClicked()
  {
    string selectedCharacter = characterDropdown.options[characterDropdown.value].text;
    ServerManager.Instance.setCharacterIndex(characterDropdown.value);
    Debug.Log("Character selected: " + characterDropdown.value);
    characterInfoText.text = selectedCharacter;
  }

  private void OnDestroy()
  {
    // Unsubscribe from the event to avoid memory leaks
    LobbyManager.OnLobbyCreated -= EnableStartButton;
    LobbyManager.OnLobbyJoined -= ShowCharacterSelectionPanel;

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