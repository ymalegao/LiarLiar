using UnityEngine;

public class ClueObject : MonoBehaviour
{
  [Header("Clue Properties")]
  [SerializeField] private string clueText; // The text of the clue
  [SerializeField] private GameObject clueUIPrompt; // UI prompt to interact

  private bool playerInRange = false;

  private void Start()
  {
    clueUIPrompt.SetActive(false);
  }

  private void Update()
  {
    if ( Input.GetKeyDown(KeyCode.R)) // Change key as needed
    {
      if(playerInRange){
        CollectClue();
      } 
    }
  }

  public void CollectClue()
  {
    JournalManager.Instance.AddClue(clueText); // Add clue to journal
    Destroy(gameObject); // Remove clue from the world after collection
  }

public void ShowPrompt(bool show)
    {
        clueUIPrompt?.SetActive(show);
    }
}
