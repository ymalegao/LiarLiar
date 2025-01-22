using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform[] waypoints; // Waypoints to assign to spawned NPCs
    [SerializeField] private Vector3 spawnPosition;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        if (npcPrefab == null)
        {
            Debug.LogError("NPC prefab is not assigned!");
            return;
        }

        // Spawn the NPC dynamically on the server
        GameObject npcInstance = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);

        // Assign waypoints to the spawned NPC
        var npcMovement = npcInstance.GetComponent<NpcMovement>();
        if (npcMovement != null)
        {
            npcMovement.SetWaypoints(waypoints);  // Make sure the waypoints are set 
        }
        else
        {
            Debug.LogError("NpcMovement component is missing from the NPC prefab!");
        }

        // Spawn the NPC on the network
        npcInstance.GetComponent<NetworkObject>().Spawn();
    }
}
