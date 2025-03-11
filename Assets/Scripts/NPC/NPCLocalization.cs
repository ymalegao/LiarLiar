using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class NPCLocalization : NetworkBehaviour, IInteractable
{
    [Header("NPC Data")]
    public string npcName;

    // Store localization keys instead of raw text
    public string[] dialogueKeys;
    public Sprite npcSprite;

    private NpcMovement npcMovement;
    private Animator animator;
    private NetworkVariable<bool> isInteracting = new NetworkVariable<bool>(false);

    private void Start()
    {
        npcMovement = GetComponent<NpcMovement>();
        animator = GetComponent<Animator>();

        if (string.IsNullOrEmpty(npcName))
        {
            Debug.LogError($"NPC on {gameObject.name} has no name set!");
        }

        if (dialogueKeys == null || dialogueKeys.Length == 0)
        {
            Debug.LogError($"NPC {npcName} has no dialogue keys assigned!");
        }

        JournalManager.Instance.RegisterNPC(npcName, npcName, npcSprite);
    }

    public void Interact()
    {
        Debug.Log("Interacting with NPC locationaliz");
        if (isInteracting.Value) return; // Prevent multiple players from interacting
        Debug.Log("Requesting interaction with NPC");
        RequestInteractServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestInteractServerRpc(ulong clientId)
    {
        if (isInteracting.Value) return;

        isInteracting.Value = true; // Lock the NPC

        if (npcMovement != null)
        {
            npcMovement.StopMovement(); // Stop movement on the server
        }

        InteractClientRpc(clientId); // Only notify the interacting player
    }

    [ClientRpc]
    private void InteractClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return; // Only execute for the correct player

        if (npcMovement != null)
        {
            npcMovement.StopMovement();
        }

        if (animator != null)
        {
            animator.SetFloat("npc_speed", 0f);
        }

        Debug.Log("Starting dialogue with NPC...");

        // Ensure dialogue starts only after stopping movement
        StartCoroutine(StartDialogueAfterMovement());
    }

    private IEnumerator StartDialogueAfterMovement()
    {
        yield return new WaitForEndOfFrame(); // Ensure movement stops first
        Debug.Log("Starting dialogue...");
        DialogueManager.Instance.StartDialogue(npcName, dialogueKeys); // Pass localization keys
        DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
    }

    private void HandleDialogueEnd()
    {
        DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        EndInteractionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndInteractionServerRpc()
    {
        isInteracting.Value = false; // Unlock NPC
        EndInteractionClientRpc();
    }

    [ClientRpc]
    private void EndInteractionClientRpc()
    {
        if (npcMovement != null)
        {
            npcMovement.ResumeMovement(); // Ensure NPC resumes movement on all clients
        }
    }
}
