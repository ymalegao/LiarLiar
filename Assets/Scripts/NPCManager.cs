using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }
    public List<GameObject> allNPCs = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterNPC(GameObject npc)
    {
        Debug.Log("yo adding this npc to our list " + npc);
        if (!allNPCs.Contains(npc))
        {
            allNPCs.Add(npc);
            Debug.Log("added npc");
            for (int i = 0; i < allNPCs.Count; i++) {
              Debug.Log("npc " + allNPCs[i]);
            }
        }
    }

    public List<GameObject> GetAllNPCs()
    {
        return allNPCs;
    }
}
