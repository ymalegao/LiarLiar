using UnityEngine;
using Unity.Netcode;

public class FakeNPCInteraction : NetworkBehaviour, IInteractable
{
    [Header("Fake NPC Data")]
    public string npcName;
    [TextArea] public string[] truthsAndLies; // 3 statements (2 truths, 1 lie)

    private bool hasInteracted;

    private void Start()
    {
        if (truthsAndLies.Length != 3)
        {
            Debug.LogError($"FakeNPC {npcName} does not have exactly 3 statements!");
        }
    }

    public void Interact()
    {
        if (!IsServer) return; // Ensure only the server handles interactions

        if (!hasInteracted)
        {
            Debug.Log($"Seeker is interacting with FakeNPC: {npcName}");
            DialogueManager.Instance.StartDialogue(npcName, truthsAndLies);
            hasInteracted = true;
        }
    }
}
