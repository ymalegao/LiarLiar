using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [TextArea]
    public string[] dialogueLines;

    private NpcMovement npcMovement;

    private void Awake()
    {
        npcMovement = GetComponent<NpcMovement>();
    }

    public void Interact()
    {
        Debug.Log("Interacting with NPC");
        npcMovement.StopMovement(); // Stop NPC movement
        DialogueManager.Instance.StartDialogue(dialogueLines);
    }

    private void OnDialogueEnd()
    {
        Debug.Log("Dialogue ended");
        npcMovement.ResumeMovement(); // Resume NPC movement
    }
}
