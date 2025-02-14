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
    if (playerInRange && Input.GetKeyDown(KeyCode.R)) // Change key as needed
    {
      CollectClue();
    }
  }

  private void CollectClue()
  {
    JournalManager.Instance.AddClue(clueText); // Add clue to journal
    Debug.Log($"Clue collected: {clueText}");
    Destroy(gameObject); // Remove clue from the world after collection
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Seeker"))
    {
      playerInRange = true;
      Debug.Log("I'm in range :)");
      if (clueUIPrompt != null)
      {
        clueUIPrompt.SetActive(true);
      }
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Seeker"))
    {
      playerInRange = false;
      if (clueUIPrompt != null)
      {
        clueUIPrompt.SetActive(false);
      }
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Seeker"))
    {
      Debug.Log("Player can now read the clue!");
      playerInRange = true;
      Debug.Log("I'm in range :)");
      if (clueUIPrompt != null)
      {
        clueUIPrompt.SetActive(true);
      }
    }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Seeker"))
    {
      Debug.Log("Player can now NOT read the clue!");
      playerInRange = false;
      Debug.Log("I'm not in range :)");
      if (clueUIPrompt != null)
      {
        clueUIPrompt.SetActive(false);
      }
    }
  }
}
