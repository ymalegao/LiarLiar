using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
  [Header("NPC Data")]
  public string npcName; // The NPC's name
  [TextArea]
  public string[] dialogueLines; // The dialogue lines for this NPC

    public Sprite npcSprite; // The NPC's sprite (Assigned in Inspector)


  private NpcMovement npcMovement;

    private Animator animator;


    private void Start()
  {
    npcMovement = GetComponent<NpcMovement>();

    animator = GetComponent<Animator>(); // Ensure animator is assigned

    if (string.IsNullOrEmpty(npcName))
    {
      Debug.LogError($"NPC on {gameObject.name} has no name set!");
    }

    if (dialogueLines == null || dialogueLines.Length == 0)
    {
      Debug.LogError($"NPC {npcName} has no dialogue lines assigned!");
    }

    JournalManager.Instance.RegisterNPC(npcName, npcName, npcSprite);
  }

  public void Interact()
  {
    // Stop NPC movement during interaction
    if (npcMovement != null)
    {
      npcMovement.StopMovement();
    }


    if (animator != null)
    {
       animator.SetFloat("npc_speed", 0f);
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
    // Resume NPC movement
    if (npcMovement != null)
    {
      npcMovement.ResumeMovement();
    }
  }
}
