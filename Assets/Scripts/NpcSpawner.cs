using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform[] waypoints;
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
        Debug.LogError("NPC prefab is null");
        return;
    }

    GameObject npcInstance = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);

    var npcMovement = npcInstance.GetComponent<NpcMovement>();
    if (npcMovement != null)
    {
        npcMovement.SetWaypoints(waypoints);
    }
    else
    {
        Debug.LogError("NpcMovement component is missing from the NPC prefab");
    }

    Debug.Log("spawning npc " + npcInstance);
    NPCManager.Instance?.RegisterNPC(npcInstance);
    
    npcInstance.GetComponent<NetworkObject>().Spawn();
  }
}
