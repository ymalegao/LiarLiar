using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSeekerSelection : MonoBehaviour
{
  [SerializeField] private GameObject seekerSelectionPanel;
  [SerializeField] private SeekerSelectionUI seekerSelectionUI;

  private void Start()
  {
    if (seekerSelectionPanel != null)
    {
      seekerSelectionPanel.SetActive(false);
    }
  }

  public void ToggleSeekerSelection()
  {
    if (seekerSelectionPanel != null)
    {
      bool isActive = !seekerSelectionPanel.activeSelf;
      seekerSelectionPanel.SetActive(isActive);

      if (isActive)
      {
        List<GameObject> allNPCs = NPCManager.Instance?.GetAllNPCs() ?? new List<GameObject>();
        Debug.Log("Grabbing all NPCs");

        List<GameObject> fakeNPCs = ServerManager.Instance.GetFakeNPCs();

        // Create a HashSet to avoid duplicates when adding fake NPCs
        HashSet<GameObject> uniqueNPCs = new HashSet<GameObject>(allNPCs);

        // Add fake NPCs to the HashSet (will automatically avoid duplicates)
        foreach (var fakeNPC in fakeNPCs)
        {
          uniqueNPCs.Add(fakeNPC);
        }

        // Convert the HashSet back to a list
        allNPCs = new List<GameObject>(uniqueNPCs);

        if (allNPCs.Count > 0 && seekerSelectionUI != null)
        {
          Debug.Log("Initializing NPC selection UI with all NPCs including fake ones.");
          seekerSelectionUI.Initialize(allNPCs);
        }
      }
    }
  }

}
