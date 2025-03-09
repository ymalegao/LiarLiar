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
using System.Linq;
using Newtonsoft.Json;

public class LobbyManager : MonoBehaviour
{
  private Lobby _hostLobby;
  public Lobby _joinLobby;
  private float _heartbeatTimer;
  private float lobbyupdateTimer;

  private bool _rolesAssigned;

  // public TMP_Text playerNamesText;

  private string playerName = "Za";
  public static event Action OnLobbyCreated;
  public static event Action OnLobbyJoined;
  public static event Action OnGameStarted;

  private System.Random _random = new System.Random();

  private const string SEEKER_ROLE = "Seeker";
  private const string FAKENPC_ROLE = "FakeNPC";

  public bool _isHost = false;
  public static LobbyManager Instance { get; private set; }

  private bool isTestingLocally = false;

  public Dictionary<ulong, string> _clientToPlayerIdMap = new Dictionary<ulong, string>();
  public Dictionary<string, string> _playerNameToPlayerIdMap = new Dictionary<string, string>();


  public HashSet<string> npcTaken = new HashSet<string>();
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
      _clientToPlayerIdMap[NetworkManager.Singleton.LocalClientId] = AuthenticationService.Instance.PlayerId;
      _playerNameToPlayerIdMap[playerName] = AuthenticationService.Instance.PlayerId;
    }
    else
    {
      Debug.LogWarning("Player is already signed in as: " + AuthenticationService.Instance.PlayerId);
    }

  }

  public string GetPlayerIdFromClientId(ulong clientId)
  {
    return _clientToPlayerIdMap.TryGetValue(clientId, out string playerId) ? playerId : null;
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
                        { "Character", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "DefaultCharacter") },
                        { "AuthID", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, AuthenticationService.Instance.PlayerId) },
                        { "Role", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "DefaultRole") }
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
      Debug.Log("Lobby created with code: " + _hostLobby.LobbyCode);
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
      if (_isHost)
      {
        _hostLobby = _joinLobby;
      }
      ulong clientId = NetworkManager.Singleton.LocalClientId;
      if (!_clientToPlayerIdMap.ContainsKey(clientId))
      {
        _clientToPlayerIdMap[clientId] = AuthenticationService.Instance.PlayerId;
        _playerNameToPlayerIdMap[playerName] = AuthenticationService.Instance.PlayerId;
      }
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

      printPlayers(_joinLobby);
    }
    catch (LobbyServiceException e)
    {
      Debug.LogError("Failed to update player data: " + e.Message);
    }
  }

  public string GetPlayerRoleFromClient(ulong clientId)
  {
    if (!_clientToPlayerIdMap.TryGetValue(clientId, out string authId))
    {
      Debug.LogWarning("Mapping not found for client " + clientId);
      return FAKENPC_ROLE;
    }
    if (!_joinLobby.Data.TryGetValue("RoleAssignments", out DataObject roleAssignments))
    {
      Debug.LogWarning("RoleAssignments not found in lobby data");
      return FAKENPC_ROLE;
    }
    var assignments = JsonConvert.DeserializeObject<Dictionary<string, RoleAssignment>>(roleAssignments.Value);
    return assignments.TryGetValue(authId, out RoleAssignment assignment) ? assignment.Role : FAKENPC_ROLE;
  }

  public int GetPlayerSpriteIndexFromClient(ulong clientId)
  {
    if (!_clientToPlayerIdMap.TryGetValue(clientId, out string authId))
    {
      Debug.LogWarning("Mapping not found for client " + clientId);
      return 0;
    }
    if (!_joinLobby.Data.TryGetValue("RoleAssignments", out DataObject roleAssignments))
    {
      Debug.LogWarning("RoleAssignments not found in lobby data");
      return 0;
    }
    var assignments = JsonConvert.DeserializeObject<Dictionary<string, RoleAssignment>>(roleAssignments.Value);
    return assignments.TryGetValue(authId, out RoleAssignment assignment) ? assignment.SpriteIndex : 0;
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
      await AssignRolesAndSprites();
      _rolesAssigned = true;

      string relayJoinCode = await ServerManager.Instance.CreateRelay();

      await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions
      {
        Data = new Dictionary<string, DataObject>
                {
                    { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode) }
                }
      });

      NetworkManager.Singleton.StartHost();
      NetworkManager.Singleton.SceneManager.LoadScene("Gameplay Functions", UnityEngine.SceneManagement.LoadSceneMode.Single);
      //Debug.Log("OnGameStarted Invoked");
      //OnGameStarted?.Invoke();
    }
    else
    {

      string relayJoinCode = _joinLobby.Data["RelayJoinCode"].Value;
      bool success = await ServerManager.Instance.JoinRelay(relayJoinCode);

      if (success)
      {
        NetworkManager.Singleton.StartClient();
      }
      else
      {
        Debug.LogError("❌ Client failed to join relay.");
        //try again
        await StartGame();
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
                { "Character", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "wizard-sheet_0") },
                { "Role", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "DefaultRole") },
                { "AuthID", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, AuthenticationService.Instance.PlayerId)}
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
        lobbyupdateTimer = 1.1f;
        try
        {
          Lobby lobby = await Lobbies.Instance.GetLobbyAsync(_joinLobby.Id);
          _joinLobby = lobby;
          foreach (Player player in _joinLobby.Players)
          {
            if (!_playerNameToPlayerIdMap.ContainsKey(player.Data["PlayerName"].Value))
            {
              _playerNameToPlayerIdMap[player.Data["PlayerName"].Value] = player.Data["AuthID"].Value;
            }
          }

          if (_joinLobby.Data.ContainsKey("RelayJoinCode") && !_isHost)
          {
            await StartGame();
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
    }
    catch (LobbyServiceException e)
    {
      Debug.LogError("Failed to query lobbies: " + e.Message);
    }
  }

  public HashSet<string> GetNpcTaken()
  {
    return npcTaken;
  }

  private async Task AssignRolesAndSprites()
  {
    var roleAssignments = new Dictionary<string, object>();
    var players = _joinLobby.Players;



    var random = new System.Random();
    var combinedList = ServerManager.Instance.characterPrefabs
        .Select((prefab, index) => new { Prefab = prefab, SpriteIndex = index })
        .OrderBy(x => random.Next())
        .ToList();

    var availablePrefabs = new Queue<string>(combinedList.Select(x => x.Prefab.name));
    var availableSprites = new Queue<int>(combinedList.Select(x => x.SpriteIndex));

    var seeker = players[_random.Next(players.Count)];
    roleAssignments[seeker.Data["AuthID"].Value] = new RoleAssignment
    {
      Role = SEEKER_ROLE,
      SpriteIndex = -1
    };

    Task.Delay(1000);


    foreach (var player in players.Where(p => p.Id != seeker.Id))
    {

      int spriteIndex = availableSprites.Count > 0 ? availableSprites.Dequeue() : _random.Next(ServerManager.Instance.characterPrefabs.Count);
      npcTaken.Add(availablePrefabs.Dequeue());

      roleAssignments[player.Data["AuthID"].Value] = new RoleAssignment
      {
        Role = FAKENPC_ROLE,
        SpriteIndex = spriteIndex
      };
    }

    await Lobbies.Instance.UpdateLobbyAsync(_joinLobby.Id, new UpdateLobbyOptions
    {
      Data = new Dictionary<string, DataObject>
            {
                { "RoleAssignments", new DataObject(DataObject.VisibilityOptions.Public, value: JsonConvert.SerializeObject(roleAssignments)) }
            }
    });
  }

  private async Task UpdatePlayerRole(string playerId, string role, int spriteIndex)
  {
    try
    {
      await LobbyService.Instance.UpdatePlayerAsync(_joinLobby.Id, playerId, new UpdatePlayerOptions
      {
        Data = new Dictionary<string, PlayerDataObject>
                {
                    { "Role", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, role) },
                    { "SpriteIndex", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, spriteIndex.ToString()) }
                }
      });
    }
    catch (Exception e)
    {
      Debug.LogError($"Failed to update player data: {e.Message}");
    }
  }

  public void ReturnToLobby()
  {
    if (NetworkManager.Singleton.IsServer)
    {
      NetworkManager.Singleton.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
  }

  public string GetPlayerRole(string playerName)
  {
    if (!_playerNameToPlayerIdMap.TryGetValue(playerName, out string playerId))
    {
      Debug.LogWarning($"⚠️ Player ID not found for player {playerName}");
      return FAKENPC_ROLE;
    }

    if (!_joinLobby.Data.TryGetValue("RoleAssignments", out DataObject roleAssignments))
    {
      Debug.LogWarning("⚠️ RoleAssignments data not found in lobby");
      return FAKENPC_ROLE;
    }

    var assignments = JsonConvert.DeserializeObject<Dictionary<string, RoleAssignment>>(roleAssignments.Value);
    return assignments.TryGetValue(playerId, out RoleAssignment assignment) ? assignment.Role : FAKENPC_ROLE;
  }

  public int GetPlayerSpriteIndex(string playerName)
  {
    if (!_playerNameToPlayerIdMap.TryGetValue(playerName, out string playerId))
    {
      Debug.LogWarning($"⚠️ Player ID not found for player {playerName}");
      return 0;
    }

    if (!_joinLobby.Data.TryGetValue("RoleAssignments", out DataObject roleAssignments))
    {
      Debug.LogWarning("⚠️ RoleAssignments data not found in lobby");
      return 0;
    }

    var assignments = JsonConvert.DeserializeObject<Dictionary<string, RoleAssignment>>(roleAssignments.Value);

    return assignments.TryGetValue(playerId, out RoleAssignment assignment) ? assignment.SpriteIndex : 0;
  }
}

public class RoleAssignment
{
  public string Role { get; set; }
  public int SpriteIndex { get; set; }
}
