using UnityEngine;

public class FakeNPC : MonoBehaviour, IInteractable
{
    [Header("Fake NPC Data")]
    public string npcName; // The NPC's name
    [TextArea]
    public string[] dialogueLines; // The dialogue lines for this NPC

    private NpcMovement npcMovement;

    private void Start()
    {
        npcMovement = GetComponent<NpcMovement>();
        if (string.IsNullOrEmpty(npcName))
    {
        Debug.LogError($"NPC on {gameObject.name} has no name set!");
    }
    else
    {
        Debug.Log($"NPC Name: {npcName}");
    }

    if (dialogueLines == null || dialogueLines.Length == 0)
    {
        Debug.LogError($"NPC {npcName} has no dialogue lines assigned!");
    }
    else
    {
        Debug.Log($"NPC {npcName} dialogue lines: {string.Join(", ", dialogueLines)}");
    }
        
        JournalManager.Instance.RegisterNPC(npcName, npcName, "farmer" , null);
        Debug.Log("NPC registered: " + npcName);
    }

    public void Interact()
    {
        Debug.Log($"Interacting with NPC: {npcName}");

        // Stop NPC movement during interaction
        if (npcMovement != null)
        {
            npcMovement.StopMovement();
        }

        // Start dialogue and pass the NPC's name
        DialogueManager.Instance.StartDialogue(npcName, dialogueLines);

        // Subscribe to DialogueManager's OnDialogueEnd event
        DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
    }

    private void HandleDialogueEnd()
    {
        // Unsubscribe from the event to avoid multiple triggers
        DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;

        Debug.Log($"Dialogue ended with NPC: {npcName}");

        // Resume NPC movement
        if (npcMovement != null)
        {
            npcMovement.ResumeMovement();
        }
    }
}
