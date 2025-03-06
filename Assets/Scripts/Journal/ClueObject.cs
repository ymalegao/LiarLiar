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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure player has the "Player" tag
        {
            playerInRange = true;
            Debug.Log("Player in range");
            ShowPrompt(true);
            other.GetComponent<SeekerInteraction>()?.SetCurrentClue(this); // Assign this clue to the player
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player in range");
            ShowPrompt(false);
            other.GetComponent<SeekerInteraction>()?.ClearCurrentClue(); // Remove clue when leaving
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Change key as needed
        {
            if (playerInRange)
            {
                CollectClue();
            }
        }
    }

    public void CollectClue()
  {
    JournalManager.Instance.AddClue(clueText); // Add clue to journal
    Destroy(gameObject); // Remove clue from the world after collection
    JournalManager.Instance.ShowNPCDetails("clues"); // Show NPC details after collecting clue
    }

public void ShowPrompt(bool show)
    {
        clueUIPrompt?.SetActive(show);
    }
}
