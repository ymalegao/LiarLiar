using UnityEngine;

public class JournalTest : MonoBehaviour
{
    private void Start()
    {
        // Add some test data to the journal
        AddTestClues();
        AddTestTruthsAndLies();

        // Simulate switching tabs
        Invoke(nameof(SwitchToTruthsTab), 2f); // Switch to truths tab after 2 seconds
        Invoke(nameof(SwitchToCluesTab), 4f); // Switch back to clues tab after 4 seconds

        // Simulate highlighting a clue after 6 seconds
        // Invoke(nameof(HighlightFirstClue), 6f);
    }

    private void AddTestClues()
    {
        JournalManager.Instance.AddClue("The door was unlocked when I arrived.");
        JournalManager.Instance.AddClue("There was a strange sound coming from the attic.");
        JournalManager.Instance.AddClue("I saw someone leaving the scene in a red car.");
    }

    private void AddTestTruthsAndLies()
    {
        JournalManager.Instance.AddTruthsAndLies("I love pizza", "I have a dog", "I climbed Mount Everest");
        JournalManager.Instance.AddTruthsAndLies("I never lie", "I love cats", "I own a mansion in Paris");
    }

    private void SwitchToTruthsTab()
    {
        Debug.Log("Switching to Truths tab...");
        JournalManager.Instance.ShowTruths();
    }

    private void SwitchToCluesTab()
    {
        Debug.Log("Switching to Clues tab...");
        JournalManager.Instance.ShowClues();
    }

    private void HighlightFirstClue()
    {
        Debug.Log("Highlighting the first clue...");
        Transform firstClue = JournalManager.Instance.cluesContent.transform.GetChild(0); // Get the first clue
        JournalManager.Instance.HighlightEntry(firstClue.gameObject); // Highlight it
    }
}
