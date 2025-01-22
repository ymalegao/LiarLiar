using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public GameObject dialoguePanel; // Assign a panel for dialogue in the UI
    public Text dialogueText; // Assign a Text component to show dialogue

    private Queue<string> dialogueQueue = new Queue<string>();
    public bool IsDialogueActive { get; private set; } = false;

    // Event triggered when dialogue ends
    public event System.Action OnDialogueEnd;


    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        dialogueQueue.Clear();

        foreach (string line in dialogueLines)
        {
            dialogueQueue.Enqueue(line);
        }

        dialoguePanel.SetActive(true);
        IsDialogueActive = true;


        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = dialogueQueue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; // Wait for a frame between each letter
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialogueActive = false;

        // Trigger the end-of-dialogue event
        OnDialogueEnd?.Invoke();
    }
}
