using UnityEngine;
using System.Collections.Generic;

public class FakeNPC : MonoBehaviour, IInteractable
{
  [Header("Fake NPC Data")]
  public string npcName; // The NPC's name
  [TextArea]
  public string[] dialogueLines; // The dialogue lines for this NPC

  private NpcMovement npcMovement;

  public Sprite npcSprite; // The NPC's sprite (Assigned in Inspector)


  private void Start()
  {
    npcMovement = GetComponent<NpcMovement>();
    if (string.IsNullOrEmpty(npcName))
    {
      Debug.LogError($"NPC on {gameObject.name} has no name set!");
    }

    if (dialogueLines == null || dialogueLines.Length == 0)
    {
      Debug.LogError($"NPC {npcName} has no dialogue lines assigned!");
    }

    if (npcSprite == null)
        {
            Debug.LogWarning($"NPC {npcName} has no sprite assigned!");
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
