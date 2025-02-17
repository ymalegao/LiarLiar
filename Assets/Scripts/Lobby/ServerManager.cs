using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using Unity.Services.Core;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class ServerManager : NetworkBehaviour
{
  public static ServerManager Instance { get; private set; }
  public NetworkVariable<ulong> seekerClientId = new NetworkVariable<ulong>();

  
  private HashSet<string> spawnedFakeNPCnames = new HashSet<string>(); // ✅ Keep track of fake NPC types


  public GameObject tempPlayerPrefab; // Temporary placeholder
  public List<GameObject> characterPrefabs; // List of all possible character prefabs
  private List<GameObject> spawnedFakeNPCs = new List<GameObject>();  // Track the actual spawned Fake NPCs
                                                                      // public hashSet<int> spritesTaken; // List of all possible roles
  public GameObject seekerPrefab; // Seeker prefab

  private GameObject prefabToUse;

  public const string SEEKER_ROLE = "Seeker";

  public const string FAKENPC_ROLE = "FakeNPC";

  public Dictionary<ulong, string> _clientAuthIdMap = new Dictionary<ulong, string>();


  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    if (NetworkManager.Singleton == null) return;
    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
  }



  private IEnumerator MapClientToAuthId(ulong clientId)
  {
    float timeout = 10f;
    float startTime = Time.time;

    while (Time.time - startTime < timeout)
    {
      if (LobbyManager.Instance._clientToPlayerIdMap.TryGetValue(clientId, out string authId))
      {
        if (!_clientAuthIdMap.ContainsKey(clientId)) // Prevent duplicates
        {
          _clientAuthIdMap[clientId] = authId;
          Debug.Log($"🔗 Mapped client {clientId} to AuthID {authId} in ServerManager");
        }
        yield break;
      }
      yield return new WaitForSeconds(0.5f);
    }

    Debug.LogError($"⌛ Failed to map client {clientId} to an AuthID");
  }







  private async void OnClientConnected(ulong clientId)
  {
    if (!IsServer) return; // ✅ Only the server should handle spawning

    Debug.Log($"🎮 Client connected: {clientId}");




    //here accesses the hsahmap to get playerID from clientID and then gets the role using the playerID



    Debug.Log($"🎮 Client connected: {clientId}");


    //here accesses the hsahmap to get playerID from clientID and then gets the role using the playerID



    // 1️⃣ Spawn a temporary player object first
    Vector3 spawnPos = new Vector3(0f, 0f, 0f);
    GameObject tempPlayer = Instantiate(tempPlayerPrefab, spawnPos, Quaternion.identity);
    tempPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    Debug.Log($"👤 Temporary player spawned for {clientId}");

    // 2️⃣ After delay, replace with the correct character prefab
    StartCoroutine(DelayedCharacterReplace(clientId));

  }



  private IEnumerator DelayedCharacterReplace(ulong clientId)
  {
    float timeout = 5f;
    float startTime = Time.time;
    // Wait until the mapping is available or timeout.
    while (!LobbyManager.Instance._clientToPlayerIdMap.ContainsKey(clientId) && Time.time - startTime < timeout)
    {
      yield return new WaitForSeconds(0.2f);
    }
    // If mapping still isn't available, log a warning (or handle it as needed).
    if (!LobbyManager.Instance._clientToPlayerIdMap.ContainsKey(clientId))
    {
      Debug.LogWarning($"Mapping still not available for client {clientId} after waiting");
    }
    //get mapping from local hashmap
    // _clientAuthIdMap.TryGetValue(clientId, out string playerId);
    //using the playerID get the role
    // string role = GetPlayerRole(clientId);        
    // Debug.Log($"👤 Player ID for {clientId}: {playerId}");

    yield return new WaitUntil(() => IsClientSceneSynchronized(clientId));
    Debug.Log($"🎮 Scene synchronized for {clientId}");
    ReplacePlayerWithCharacter(clientId);
  }


  private bool IsClientSceneSynchronized(ulong clientId)
  {
    if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
    {
      Debug.LogError($"Client {clientId} not connected.");
      return false;
    }
    var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    if (activeScene != "Gameplay Functions")
    {
      Debug.Log($"🎮 Scene not synchronized for {clientId}");
      return false;
    }
    NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
    if (playerObject == null)
    {
      Debug.LogError($"⚠️ No PlayerObject found for {clientId}");
      return false;
    }

    return playerObject.IsSpawned && playerObject != null;
  }

  private void ReplacePlayerWithCharacter(ulong clientId)
  {
    string role = LobbyManager.Instance.GetPlayerRoleFromClient(clientId);
    Debug.Log($"👤 Role for in ReplacePlayerWithCharacter {clientId}: {role}");
    if (role == "Seeker")
    {
      Debug.Log("👤 Role is Seeker");
      seekerClientId.Value = clientId;
      Debug.Log("👤 Seeker client id is " + seekerClientId.Value);
    }
    else
    {
      Debug.Log("👤 Role is not Seeker");
    }

    int indexforPrefab = LobbyManager.Instance.GetPlayerSpriteIndexFromClient(clientId);
    Debug.Log($"🛑 Sprite Index for client {clientId}: {indexforPrefab}");

    // For testing, we choose the seekerPrefab for "Seeker" or the first character for FakeNPC.
    // You can expand this logic to use the sprite index if needed:
    prefabToUse = role == SEEKER_ROLE ? seekerPrefab : characterPrefabs[indexforPrefab];

    if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
    {
      Debug.LogError($"Client {clientId} not connected.");
      return;
    }
    NetworkObject oldPlayerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

    if (oldPlayerObject == null)
    {
      Debug.LogError($"⚠️ No PlayerObject found for {clientId}");
      return;
    }

    Vector3 spawnPos = oldPlayerObject.transform.position; // Keep same position
    Debug.Log($"🎮 Spawning character for {clientId} at {spawnPos}");
    oldPlayerObject.Despawn();
    Destroy(oldPlayerObject.gameObject);
    Debug.Log($"🎮 Destroyed old player object for {clientId}");
    // Debug.Log("Did not destroy old player object");
    Debug.Log("prefato use name is " + prefabToUse.name);
    GameObject newCharacter = Instantiate(prefabToUse, spawnPos, Quaternion.identity);
    if (role == FAKENPC_ROLE)
    {
      ServerManager.Instance.RegisterFakeNPC(newCharacter);
    }

    Debug.Log($"🔍 Index for client {clientId}: {indexforPrefab}");
    Debug.Log($"🎮 Instantiated new character for {clientId}");
    newCharacter.tag = role; // Optionally tag the object for debugging
    newCharacter.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    Debug.Log($"🎮 Spawned character for {clientId} with role {role}");
  }
  public async Task<string> CreateRelay()
  {
    try
    {
      Allocation allocation = await Relay.Instance.CreateAllocationAsync(4);
      string joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

      UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
      transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                                  allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

      return joinCode;
    }
    catch (Exception e)
    {
      Debug.LogError($"Relay creation failed: {e.Message}");
      return null;
    }
  }

  //

  public async Task<bool> JoinRelay(string relayJoinCode)
  {
    try
    {
      JoinAllocation joinAllocation = await Relay.Instance.JoinAllocationAsync(relayJoinCode);

      UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
      transport.SetRelayServerData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port,
                                  joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData,
                                  joinAllocation.HostConnectionData);

      return true;
    }
    catch (Exception e)
    {
      Debug.LogError($"Relay join failed: {e.Message}");
      return false;
    }
  }

  public string GetPlayerRole(ulong clientId)
  {
    if (!_clientAuthIdMap.TryGetValue(clientId, out string playerId))
    {
      Debug.LogWarning($"⚠️ Player ID not found for client {clientId}");
      return FAKENPC_ROLE;
    }

    if (!LobbyManager.Instance._joinLobby.Data.TryGetValue("RoleAssignments", out DataObject roleAssignments))
    {
      Debug.LogWarning("⚠️ RoleAssignments data not found in lobby");
      return FAKENPC_ROLE;
    }

    var assignments = JsonConvert.DeserializeObject<Dictionary<string, RoleAssignment>>(roleAssignments.Value);

    return assignments.TryGetValue(playerId, out RoleAssignment assignment) ? assignment.Role : FAKENPC_ROLE;
  }

  //     [ServerRpc(RequireOwnership = false)]
  // public void UpdateMappingServerRpc(ulong clientId, string authId, ServerRpcParams rpcParams = default)
  // {
  //     if (!_clientAuthIdMap.ContainsKey(clientId))
  //     {
  //         _clientAuthIdMap[clientId] = authId;
  //         Debug.Log($"Server: Mapped client {clientId} to AuthID {authId}");
  //     }
  // }
  [ServerRpc(RequireOwnership = false)]
  public void UpdateMappingServerRpc(ulong clientId, string authId, ServerRpcParams rpcParams = default)
  {
    // Update the LobbyManager's mapping (which is used for role lookups)
    if (!LobbyManager.Instance._clientToPlayerIdMap.ContainsKey(clientId))
    {
      LobbyManager.Instance._clientToPlayerIdMap[clientId] = authId;
      Debug.Log($"Server: Mapped client {clientId} to AuthID {authId} in LobbyManager");
    }
  }

  // Register a Fake NPC when it's spawned
  public void RegisterFakeNPC(GameObject fakeNPC)
  {
    string npcType = fakeNPC.name.Replace("(Clone)", "").Trim();  // Get the NPC type
    if (!spawnedFakeNPCs.Contains(fakeNPC))
    {
      if (!spawnedFakeNPCnames.Contains(npcType))
      {
        spawnedFakeNPCnames.Add(npcType);
        Debug.Log($"✅ Registered Fake NPC type: {npcType}");
      }
      spawnedFakeNPCs.Add(fakeNPC);
      Debug.Log($"✅ Registered Fake NPC: {fakeNPC.name} | Instance ID: {fakeNPC.GetInstanceID()}");
    }
  }

  // Get the actual spawned Fake NPCs
  public List<GameObject> GetFakeNPCs()
  {
    return new List<GameObject>(spawnedFakeNPCs);  // Return a copy of the list of spawned Fake NPCs
  }

  
  public bool IsFakeNPCSpawned(string npcType)
    {
        return spawnedFakeNPCnames.Contains(npcType);
    }

    // ✅ Provide a method for NpcSpawner to check before spawning
    public HashSet<string> GetFakeNPCnames()
    {
        return new HashSet<string>(spawnedFakeNPCnames); // Return a copy
    }

  public GameObject GetCharacterPrefab(int index)
  {
    Debug.Log($"CharacterPrefabs.Count: {characterPrefabs.Count}");
    foreach (var prefab in characterPrefabs)
    {
      Debug.Log($"📦 CharacterPrefab: {prefab.name}");
    }

    if (index < 0 || index >= characterPrefabs.Count)
    {
      Debug.LogError($"❌ Invalid index {index} for character prefabs.");
      return null;
    }
    return characterPrefabs[index];
  }





}
