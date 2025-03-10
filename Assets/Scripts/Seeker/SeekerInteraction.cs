using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class SeekerInteraction : NetworkBehaviour
{
    public float interactionRange = 1.5f;
    public LayerMask interactableLayer;
    private ClueObject currentClue = null;
    private bool canAdvanceDialogue = true;

    private void Start()
    {
        if (!IsOwner) 
        {
            enabled = false;  // Disable input processing for non-local players
        }
    }

    private void Update()
    {
        if (!IsOwner) return; // Ensure only the local player processes input

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                if (!canAdvanceDialogue) return;
                canAdvanceDialogue = false;
                StartCoroutine(ResetDialogueAdvanceCooldown());

                DialogueManager.Instance.DisplayNextLine();
            }
            else
            {
                CheckForInteractable();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentClue != null)
        {
            Debug.Log("Collecting clue");
            currentClue.CollectClue();
            currentClue = null;
        }
    }

    private IEnumerator ResetDialogueAdvanceCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        canAdvanceDialogue = true;
    }

    private void CheckForInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);

        if (hits.Length == 0)
        {
            Debug.Log("No interactable objects detected.");
            return;
        }

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log($"Interacting with: {hit.gameObject.name}");
                interactable.Interact();
                return; // Stop after finding a valid interactable
            }
        }

        Debug.Log("No valid interactable objects found.");
    }


    public void SetCurrentClue(ClueObject clue)
    {
        currentClue = clue;
    }

    public void ClearCurrentClue()
    {
        currentClue = null;
    }
}
