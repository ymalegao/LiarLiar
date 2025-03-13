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
    [SerializeField] private GameObject journalUI;
    [SerializeField] private AudioSource buttonAudioSource; 


    private void Start()
  {
    if (seekerSelectionPanel != null)
    {
      seekerSelectionPanel.SetActive(false);
    }
  }

  public void ToggleSeekerSelection()
  {

        if (buttonAudioSource != null)
        {
            buttonAudioSource.Play(); // Play button sound
        }
        
        if (seekerSelectionPanel != null)
    {
      bool isActive = !seekerSelectionPanel.activeSelf;
      seekerSelectionPanel.SetActive(isActive);

      if (dialogueUIPanel != null)
      {
        dialogueUIPanel.SetActive(!isActive);
      }

            if (journalUI != null)
            {
                if (isActive) journalUI.SetActive(false); // Hide journal when seeker selection is active
            }


            if (buttonText != null)
      {
        buttonText.text = isActive ? "Close Selection" : "Open Selection";
      }

      if (isActive)
      {
        List<GameObject> allNPCs = NPCManager.Instance?.GetAllNPCs() ?? new List<GameObject>();

        if (allNPCs.Count > 0 && seekerSelectionUI != null)
        {
          seekerSelectionUI.Initialize(allNPCs);
        }
      }
    }
  }

}
