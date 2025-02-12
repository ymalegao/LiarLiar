using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
  [SerializeField] private GameObject[] npcPrefabs; // Array of NPC prefabs
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

      npcInstance.GetComponent<NetworkObject>().Spawn();
    }
  }
}
