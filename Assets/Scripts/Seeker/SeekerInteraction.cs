using UnityEngine;

public class SeekerInteraction : MonoBehaviour
{
  public float interactionRange = 1.5f; // Range for interaction
  public LayerMask interactableLayer; // Assign in the inspector to detect interactables
  private ClueObject currentClue = null;

  private void Update()
  {

    if (Input.GetKeyDown(KeyCode.E)) // Press E to interact
    {
      // If dialogue is active, advance to the next line
      if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
      {
        DialogueManager.Instance.DisplayNextLine();
      }
      else
      {
        // Otherwise, check for interactable objects
        CheckForInteractable();
      }
    }

    if (Input.GetKeyDown(KeyCode.R) && currentClue != null)
        {
            currentClue.CollectClue();
            currentClue = null; // Prevents multiple collections
        }
  }

  private void CheckForInteractable()
  {
    Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);

    if (hit != null)
    {
      Debug.Log($"Hit object: {hit.name}");
      IInteractable interactable = hit.GetComponent<IInteractable>();
      if (interactable != null)
      {
        Debug.Log($"Interactable found: {hit.name}");
        interactable.Interact();
      }
      else
      {
        Debug.LogWarning($"Object {hit.name} does not implement IInteractable.");
      }
    }
    else
    {
      Debug.Log("No interactable object detected.");
    }
  }

  private void OnDrawGizmosSelected()
  {
    // Visualize interaction range
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, interactionRange);
  }

  private void OnTriggerEnter2D(Collider2D other)
    {
        ClueObject clue = other.GetComponent<ClueObject>();
        if (clue != null)
        {
            currentClue = clue;
            clue.ShowPrompt(true);
        }
    }

  private void OnTriggerExit2D(Collider2D other)
    {
        if (currentClue != null && other.GetComponent<ClueObject>() == currentClue)
        {
            currentClue.ShowPrompt(false);
            currentClue = null;
        }
    }
}