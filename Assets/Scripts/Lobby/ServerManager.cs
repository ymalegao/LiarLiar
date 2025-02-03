// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;
// using Unity.Services.Authentication;
// using Unity.Services.Lobbies;


// public class ServerManager : NetworkBehaviour
// {
//     public static ServerManager Instance { get; private set; }

//     public GameObject playerPrefab; // ✅ Assign the player prefab in Unity Inspector
//     public Transform[] spawnPoints; // ✅ Assign spawn points in Unity Inspector

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void Start()
//     {
//         if (NetworkManager.Singleton == null) return;

//         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
//     }

//     private void OnClientConnected(ulong clientId)
//     {
//         if (!IsServer) return; // ✅ Only the host should spawn players

//         Debug.Log($"🎮 Client connected: {clientId}");

//         // string selectedCharacter = LobbyManager.Instance.GetPlayerCharacter(clientId);
//         // SpawnPlayer(clientId, selectedCharacter);
//         StartCoroutine(DelayedPlayerSpawn(clientId));

//     }

//     private IEnumerator DelayedPlayerSpawn(ulong clientId)
//     {
//         float timeout = 5f;
//         float timer = 0f;

//         while (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId) || 
//            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject == null)
//     {
//         if (timer > timeout)
//         {
//             Debug.LogError($"Timeout: PlayerObject not assigned for client {clientId}");
//             yield break;
//         }

//         Debug.Log($"Waiting for PlayerObject for client {clientId}...");
//         timer += 0.5f;
//         yield return new WaitForSeconds(0.5f);
//     }

//     string selectedCharacter = LobbyManager.Instance.GetPlayerCharacter(clientId);
//     SpawnPlayer(clientId, selectedCharacter);

//     }

    
//     private void SpawnPlayer(ulong clientId, string characterId)
// {
//     Debug.Log($"🎮 Spawning player {clientId} with character: {characterId}");
    
//     Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

//     GameObject characterPrefab = LobbyManager.Instance.GetCharacterPrefab(characterId);
//     if (characterPrefab == null)
//     {
//         Debug.LogError($"❌ No prefab found for character {characterId}. Spawning default.");
//         characterPrefab = playerPrefab;
//     }

//     GameObject playerInstance = Instantiate(characterPrefab, spawnPoint, Quaternion.identity);
//     playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

//     SetSprite setSprite = playerInstance.GetComponent<SetSprite>();
//     if (setSprite != null)
//     {
//         setSprite.SetCharacter(characterId);
//     }
//     else
//     {
//         Debug.LogError("SetSprite component not found on player prefab");
//     }
// }

// }
