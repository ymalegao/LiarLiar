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
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit != null)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
            else
            {
                Debug.LogWarning($"Object {hit.name} does not implement IInteractable.");
            }
        }
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
