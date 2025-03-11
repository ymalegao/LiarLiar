using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class NPC : NetworkBehaviour, IInteractable
{
    [Header("NPC Data")]
    public string npcName;
    [TextArea]
    public string[] dialogueLines;
    public Sprite npcSprite;
    public AudioClip[] dialogueAudioClips;

    private NpcMovement npcMovement;
    private Animator animator;
    private AudioSource audioSource; 
    private NetworkVariable<bool> isInteracting = new NetworkVariable<bool>(false);

    private void Start()
    {
        npcMovement = GetComponent<NpcMovement>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

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
        Debug.Log("Interacting with NPC");
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

        Debug.Log("Starting dialogue with nPC...");

        // Ensure dialogue starts only after stopping movement
        StartCoroutine(StartDialogueAfterMovement());
    }

    private IEnumerator StartDialogueAfterMovement()
    {
        yield return new WaitForEndOfFrame(); // Ensure movement stops first
        Debug.Log("Starting dialogue...");
        DialogueManager.Instance.StartDialogue(npcName, dialogueLines);
        DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;

        StartCoroutine(PlayDialogueAudio());

    }


    private IEnumerator PlayDialogueAudio()
    {
        Debug.Log($"[NPC] {npcName}: PlayDialogueAudio() started.");

        if (audioSource == null)
        {
            Debug.LogError($"[NPC] {npcName}: AudioSource is NULL in PlayDialogueAudio()!");
            yield break;
        }

        if (dialogueAudioClips == null)
        {
            Debug.LogError($"[NPC] {npcName}: dialogueAudioClips array is NULL in PlayDialogueAudio()!");
            yield break;
        }

        if (dialogueAudioClips.Length == 0)
        {
            Debug.LogWarning($"[NPC] {npcName}: dialogueAudioClips array is EMPTY, skipping audio playback.");
            yield break;
        }

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            if (i >= dialogueAudioClips.Length)
            {
                Debug.LogWarning($"[NPC] {npcName}: No audio clip for dialogue line {i}, skipping.");
                yield return new WaitForSeconds(2.0f);
                continue;
            }

            if (dialogueAudioClips[i] == null)
            {
                Debug.LogError($"[NPC] {npcName}: dialogueAudioClips[{i}] is NULL!");
                yield return new WaitForSeconds(2.0f);
                continue;
            }

            Debug.Log($"[NPC] {npcName}: Playing audio clip {i} - {dialogueAudioClips[i].name}");
            audioSource.clip = dialogueAudioClips[i];
            audioSource.Play();

            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }

        Debug.Log($"[NPC] {npcName}: Finished playing audio.");
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
