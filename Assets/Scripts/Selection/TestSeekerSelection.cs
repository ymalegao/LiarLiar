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
                // Get all NPCs and send them to the UI
                List<GameObject> allNPCs = NPCManager.Instance?.GetAllNPCs();
                Debug.Log("grabbing all npcs");
                if (allNPCs != null && seekerSelectionUI != null)
                {
                    Debug.Log("initalizing npcs");
                    seekerSelectionUI.Initialize(allNPCs);
                }
            }
        }
    }
}
