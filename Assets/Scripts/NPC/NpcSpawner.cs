using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NpcSpawner : NetworkBehaviour
{
    [Header("NPC Prefabs")]
    public GameObject[] npcPrefabs; // ✅ List of NPC prefabs to spawn

    [Header("FakeNPC to NPC Mapping")]
    public List<FakeNPCMapping> fakeNPCMappings; // ✅ Manually map in Unity Inspector

    private Dictionary<string, string> fakeNPCtoNPCMap = new Dictionary<string, string>(); // ✅ Internal dictionary

    private HashSet<string> fakeNPCs = new HashSet<string>(); // ✅ Store Fake NPCs from ServerManager

    private void Awake()
    {
        // ✅ Convert the list into a Dictionary for quick lookup
        foreach (var mapping in fakeNPCMappings)
        {
            fakeNPCtoNPCMap[mapping.fakeNPCName] = mapping.realNPCName;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return; // ✅ Only the server spawns NPCs

        Debug.Log("Checking which FakeNPCs are already spawned...");
        fakeNPCs = LobbyManager.Instance.GetNpcTaken(); // ✅ Get FakeNPCs before spawning NPCs
        Debug.Log($"👀 FakeNPCs already spawned: {string.Join(", ", fakeNPCs)}");

        SpawnNPCs();
    }

    private void SpawnNPCs()
    {
        foreach (GameObject npcPrefab in npcPrefabs)
        {
            string npcType = npcPrefab.name; // ✅ Get the NPC type

            // ✅ Check if an NPC has a FakeNPC equivalent
            if (fakeNPCtoNPCMap.ContainsValue(npcType) && fakeNPCs.Contains(GetFakeNPCName(npcType)))
            {
                Debug.Log($"❌ Skipping NPC spawn: {npcType} (FakeNPC exists)");
                continue;
            }

            // ✅ If no FakeNPC exists, spawn the NPC
            GameObject npc = Instantiate(npcPrefab, Vector3.zero, Quaternion.identity);
            npc.GetComponent<NetworkObject>().Spawn();
            Debug.Log($"✅ Spawned NPC: {npcType}");
        }
    }

    private string GetFakeNPCName(string npcName)
    {
        foreach (var mapping in fakeNPCtoNPCMap)
        {
            if (mapping.Value == npcName)
            {
                return mapping.Key;
            }
        }
        return npcName; // Return the same if no FakeNPC mapping exists
    }
}

// ✅ Custom Serializable Class for Mapping FakeNPC → NPC
[System.Serializable]
public class FakeNPCMapping
{
    public string fakeNPCName;
    public string realNPCName;
}
