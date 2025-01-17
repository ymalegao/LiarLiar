using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [TextArea]
    public string[] dialogueLines;

    public void Interact()
    {
        Debug.Log("Interacting with NPC");
        DialogueManager.Instance.StartDialogue(dialogueLines);
    }
}
