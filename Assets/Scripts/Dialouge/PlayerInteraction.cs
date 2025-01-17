using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 1.5f; // Range for interaction
    public LayerMask interactableLayer; // Assign in the inspector to detect interactables

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press E to interact
        {
            CheckForInteractable();
        }

        // Trigger DisplayNextLine in DialogueManager with the Right Arrow Key
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.DisplayNextLine();
            }
        }
    }

    private void CheckForInteractable()
{
    // Perform a raycast to detect interactable objects
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, interactionRange, interactableLayer);

    if (hit.collider != null)
    {
        Debug.Log($"Hit object: {hit.collider.name}");
        IInteractable interactable = hit.collider.GetComponent<IInteractable>();
        if (interactable != null)
        {
            Debug.Log($"Interactable found: {hit.collider.name}");
            interactable.Interact();
        }
        else
        {
            Debug.LogWarning($"Object {hit.collider.name} does not implement IInteractable.");
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
}
