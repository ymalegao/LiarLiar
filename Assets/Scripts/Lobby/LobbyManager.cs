using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using System;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    private Lobby _hostLobby;
    private Lobby _joinLobby;
    private float _heartbeatTimer;
    private float lobbyupdateTimer;

    public TMP_Text playerNamesText;

    private string playerName = "Za";
    public static event Action OnLobbyCreated;
    public static event Action OnLobbyJoined;

    public bool _isHost = false;
    public static LobbyManager Instance { get; private set; }

    private bool isTestingLocally = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate LobbyManager detected and destroyed!");
            Destroy(gameObject);
            return;
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            playerName = "Za" + UnityEngine.Random.Range(1000, 9999);
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        }
        else
        {
            Debug.Log("Player is already signed in as: " + AuthenticationService.Instance.PlayerId);
        }

        Debug.Log("Player Name: " + playerName);
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async Task HandleLobbyHeartbeat()
    {
        if (_hostLobby != null)
        {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer < 0f)
            {
                _heartbeatTimer = 15f;
                try
                {
                    await Lobbies.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogError("Failed to heartbeat lobby: " + e.Message);
                }
            }
        }
    }

    public async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)},
                        { "Character", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "DefaultCharacter") }
                    }
                },
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "DefaultMode") }
                }
            };

            _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            _joinLobby = _hostLobby;
            _isHost = true;
            Debug.Log($"Lobby created! Lobby Code: {_hostLobby.LobbyCode}");
            OnLobbyCreated?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }

    public async Task JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            _joinLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinOptions);
            Debug.Log($"Joined lobby with code: {_joinLobby.LobbyCode}");
            OnLobbyJoined?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to join lobby: " + e.Message);
        }
    }

    public async Task UpdatePlayerCharacter(string selectedCharacter)
    {
        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(_joinLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "Character", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, selectedCharacter) }
                }
            });

            Debug.Log($"Updated player character to: {selectedCharacter}");
            printPlayers(_joinLobby);

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to update player data: " + e.Message);
        }
    }

    public string GetPlayerCharacter(ulong clientId)
    {
        if (_joinLobby == null)
        {
            Debug.LogWarning("⚠️ Lobby data not available. Returning default character.");
            return "DefaultCharacter";
        }

        string clientIdString = clientId.ToString();

        foreach (Player player in _joinLobby.Players)
        {
            if (player.Data.ContainsKey("Character"))
            {
                Debug.Log($"✅ Found character for client {clientId}: {player.Data["Character"].Value}");
                return player.Data["Character"].Value;
            }
        }

        Debug.LogWarning($"⚠️ Character not found for {clientIdString}. Returning DefaultCharacter.");
        return "DefaultCharacter";
    }

    public async Task StartGame()
    {
        if (_isHost)
    {
        Debug.Log("Host is starting the game...");
        string relayJoinCode = await ServerManager.Instance.CreateRelay();
        
        // Update lobby with relay code
        await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { 
                    "RelayJoinCode", 
                    new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode) 
                }
            }
        });

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("map-creation", 
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    // Clients join using relay code from lobby data
    else
    {
        Debug.Log("Client joining game...");
        string relayJoinCode = _joinLobby.Data["RelayJoinCode"].Value;
        bool success = await ServerManager.Instance.JoinRelay(relayJoinCode);
        
        if (success)
        {
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.LogError("❌ Client failed to join relay.");
        }
    }
}


    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
                { "Character", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "wizard-sheet_0") }
            }
        };
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (_joinLobby != null)
        {
            lobbyupdateTimer -= Time.deltaTime;
            if (lobbyupdateTimer <= 0)
            {
                lobbyupdateTimer = 1.1f; // Faster polling when game starts
                try
                {
                    Lobby lobby = await Lobbies.Instance.GetLobbyAsync(_joinLobby.Id);
                    _joinLobby = lobby;
                    printPlayers(_joinLobby);

                    // 🔥 NEW: Detect if the game has started
                    if (_joinLobby.Data.ContainsKey("RelayJoinCode") && !_isHost)
                    {
                        Debug.Log("🚀 Game started by host! Joining relay...");
                        await StartGame(); // Clients auto-join when relay code appears
                    }
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogError("Failed to update lobby: " + e.Message);
                }
            }
        }
    }

    private void printPlayers(Lobby lobby)
    {
        string playerNames = "";
        foreach (Player player in lobby.Players)
        {
            playerNames += player.Data["PlayerName"].Value + "\n" + player.Data["Character"].Value + "\n";
        }
        playerNamesText.text = playerNames;
    }

    public string GetLobbyCode()
    {
        if (isTestingLocally)
        {
            return "No Lobby Code Available";
        }
        return _hostLobby != null ? _hostLobby.LobbyCode : "No Lobby Code Available";
    }

    public async void ListLobbies()
    {
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            foreach (Lobby lobby in response.Results)
            {
                Debug.Log("Lobby: " + lobby.LobbyCode);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to query lobbies: " + e.Message);
        }
    }



}
