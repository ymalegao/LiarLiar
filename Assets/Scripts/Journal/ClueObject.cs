using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

public class ClueObject : MonoBehaviour
{
    [Header("Clue Properties")]
    [SerializeField] private string clueKey; // The localization key for the clue

    private bool isCollected = false;

    [SerializeField] private GameObject clueUIPrompt; // UI prompt to interact

    private bool playerInRange = false;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound; // Assign this in Inspector
    private AudioSource audioSource;


    private void Start()
    {
        clueUIPrompt.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure player has the "Player" tag
        {
            playerInRange = true;
            Debug.Log("Player in range");
            ShowPrompt(true);
            other.GetComponent<SeekerInteraction>()?.SetCurrentClue(this); // Assign this clue to the player
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player out of range");
            ShowPrompt(false);
            other.GetComponent<SeekerInteraction>()?.ClearCurrentClue(); // Remove clue when leaving
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Change key as needed
        {
            if (playerInRange)
            {
                CollectClue();
            }
        }
    }

    public void CollectClue()
    {
        if (isCollected) return; // Prevent duplicate collection
        isCollected = true;

        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }


        StartCoroutine(LoadLocalizedClue()); // Fetch localized text before storing it

        Destroy(gameObject, pickupSound.length);

    }

    private IEnumerator LoadLocalizedClue()
    { 
        var localizedString = new LocalizedString("CluesTableReal", clueKey);
        var asyncOperation = localizedString.GetLocalizedStringAsync();

        yield return asyncOperation;

        string localizedClueText = asyncOperation.Result;
        Debug.Log($"Collected Clue: {localizedClueText}");

        JournalManager.Instance.AddClue(localizedClueText); // Add localized clue to journal
        //Destroy(gameObject); // Remove clue from the world after collection
        JournalManager.Instance.ShowNPCDetails("clues"); // Show NPC details after collecting clue
    }

    public void ShowPrompt(bool show)
    {
        clueUIPrompt?.SetActive(show);
    }
}
