using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject[] npcPrefabs; // Array of NPC prefabs
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Vector3[] spawnPositions; // Array of spawn positions

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Ensure only the server spawns NPCs
        {
            SpawnNPCs();
        }
    }

    private void SpawnNPCs()
    {
        if (npcPrefabs.Length == 0)
        {
            Debug.LogError("No NPC prefabs assigned!");
            return;
        }

        for (int i = 0; i < spawnPositions.Length; i++)
        {   
            Debug.Log(npcPrefabs);
            int prefabIndex = i % npcPrefabs.Length; // Ensure index is within range
            GameObject npcInstance = Instantiate(npcPrefabs[prefabIndex], spawnPositions[i], Quaternion.identity);

            var npcMovement = npcInstance.GetComponent<NpcMovement>();
            if (npcMovement != null)
            {
                npcMovement.SetWaypoints(waypoints);
            }
            else
            {
                Debug.LogError($"NpcMovement component is missing from {npcPrefabs[prefabIndex].name}");
            }

            npcInstance.GetComponent<NetworkObject>().Spawn();
        }
    }
 }
// using Unity.Netcode;
// using UnityEngine;

// public class NPCSpawner : NetworkBehaviour
// {
//     [SerializeField] private GameObject npcPrefab;
//     [SerializeField] private Transform[] waypoints;
//     [SerializeField] private Vector3 spawnPosition;

//     public override void OnNetworkSpawn()
//     {
//         if (IsServer)
//         {
//             SpawnNPC();
//         }
//     }

//     private void SpawnNPC()
//     {
//         if (npcPrefab == null)
//         {
//             Debug.LogError("NPC prefab is null");
//             return;
//         }

//         GameObject npcInstance = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);

//         var npcMovement = npcInstance.GetComponent<NpcMovement>();
//         if (npcMovement != null)
//         {
//             npcMovement.SetWaypoints(waypoints);
//         }
//         else
//         {
//             Debug.LogError("NpcMovement component is missing from the NPC prefab");
//         }
//         npcInstance.GetComponent<NetworkObject>().Spawn();
//     }
// }
