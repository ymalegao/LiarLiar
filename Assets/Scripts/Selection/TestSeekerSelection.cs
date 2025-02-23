using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestSeekerSelection : MonoBehaviour
{
  [SerializeField] private GameObject seekerSelectionPanel;
  [SerializeField] private SeekerSelectionUI seekerSelectionUI;
  [SerializeField] private TextMeshProUGUI buttonText;
  [SerializeField] private GameObject dialogueUIPanel;

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

      if (dialogueUIPanel != null)
      {
        dialogueUIPanel.SetActive(!isActive);
      }

      if (buttonText != null)
      {
        buttonText.text = isActive ? "Close Selection" : "Open Selection";
      }

      if (isActive)
      {
        List<GameObject> allNPCs = NPCManager.Instance?.GetAllNPCs() ?? new List<GameObject>();
        Debug.Log($"Initializing NPC selection UI with {allNPCs.Count} NPCs");

        if (allNPCs.Count > 0 && seekerSelectionUI != null)
        {
          seekerSelectionUI.Initialize(allNPCs);
        }
      }
    }
  }

}
