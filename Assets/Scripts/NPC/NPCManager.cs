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
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public void RegisterNPC(GameObject npc)
  {
    if (npc == null)
    {
      Debug.LogError("Attempted to register null NPC");
      return;
    }

    if (!allNPCs.Contains(npc))
    {
      allNPCs.Add(npc);
    }
  }

  public List<GameObject> GetAllNPCs()
  {
    if (allNPCs.Count == 0)
    {
      Debug.LogWarning("GetAllNPCs called but no NPCs are registered!");
    }
    return new List<GameObject>(allNPCs);
  }

  public void ClearNPCs()
  {
    allNPCs.Clear();
  }
}
