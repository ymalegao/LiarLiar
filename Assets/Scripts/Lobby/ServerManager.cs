using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class ServerManager : NetworkBehaviour
{
    public static ServerManager Instance { get; private set; }

    public GameObject tempPlayerPrefab; // Temporary placeholder
    public List<GameObject> characterPrefabs; // List of all possible character prefabs

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

    private async void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return; // ✅ Only the server should handle spawning

        Debug.Log($"🎮 Client connected: {clientId}");

        // 1️⃣ Spawn a temporary player object first
        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0f, UnityEngine.Random.Range(-3f, 3f));
        GameObject tempPlayer = Instantiate(tempPlayerPrefab, spawnPos, Quaternion.identity);
        tempPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        Debug.Log($"👤 Temporary player spawned for {clientId}");

        // 2️⃣ After delay, replace with the correct character prefab
        StartCoroutine(DelayedCharacterReplace(clientId));
        
    }

    private IEnumerator DelayedCharacterReplace(ulong clientId)
    {
        yield return new WaitForSeconds(2f); // Wait to ensure lobby data sync

        // 1️⃣ Get the correct character ID from `LobbyManager`
        string characterId = LobbyManager.Instance.GetPlayerCharacter(clientId);
        Debug.Log($"👤 Character ID for {clientId}: {characterId}");

        GameObject characterPrefab = characterPrefabs.Find(p => p.name == characterId);
        if (characterPrefab == null)
        {
            Debug.LogError($"❌ No prefab found for character {characterId}. Using DefaultCharacter.");
            characterPrefab = characterPrefabs[0]; // Default fallback
        }

        Debug.Log($"👤 Character prefab for {clientId}: {characterPrefab.name}");

        // 2️⃣ Replace the temporary player object with the correct character
        ReplacePlayerWithCharacter(clientId, characterPrefab);
    }


    private void ReplacePlayerWithCharacter(ulong clientId, GameObject characterPrefab)
    {
        Debug.Log($"🔄 Replacing temporary player with chadddracter for {clientId}");
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)) return;
        NetworkObject oldPlayerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        
        if (oldPlayerObject == null)
        {
            Debug.LogError($"⚠️ No PlayerObject found for {clientId}");
            return;
        }

        Vector3 spawnPos = oldPlayerObject.transform.position; // Keep same position

        oldPlayerObject.Despawn();
        Destroy(oldPlayerObject.gameObject);
        Debug.Log($"🔄 Replacing temporary player with character for {clientId}");

        GameObject newCharacter = Instantiate(characterPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"👤 Character spawned for {clientId}");
        newCharacter.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        Debug.Log($"🎮 PlayerObject spawned for {clientId}");
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
}
