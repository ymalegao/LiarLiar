using UnityEngine;
using TMPro;

public class EggSpawner : MonoBehaviour
{
    public GameObject eggPrefab;
    public Transform[] spawnPoints;
    public static EggSpawner Instance;
    public float spawnRate = 2f;
    public GameObject miniGameCanvas;
    public GameObject victoryPanel;
    public GameObject instructionsPanel;

    public GameObject failurePanel;
    public TMP_Text instructionsText;
    public int minScoreToComplete = 10;

    private void Start()
    {
        miniGameCanvas.SetActive(false); // Ensure the mini-game starts hidden
        instructionsPanel.SetActive(true); // Show instructions first
        instructionsText.text = "Welcome to Egg Drop!\n\nPress A and D to move the basket and catch eggs.\n\nYou need a score of 10 to pass.\n\nPress Play to start!";
    }

    private void Awake(){
        Instance = this;
    }

    public void StartMiniGame()
    {
        instructionsPanel.SetActive(false); // Hide instructions
        miniGameCanvas.SetActive(true);
        failurePanel.SetActive(false);
        InvokeRepeating(nameof(SpawnEgg), 1f, spawnRate);
        EggGameManager.Instance.StartGame();
    }

    public void exitGame()
    {
        EggGameManager.Instance.ResetState();
        miniGameCanvas.SetActive(false);
        instructionsPanel.SetActive(false);
        failurePanel.SetActive(false);
        victoryPanel.SetActive(false);
        EggGameManager.Instance.endGame();
        //trigger some condition if you want here using like a fakeNPC powerup Manger
        CancelInvoke(nameof(SpawnEgg));
    }

    public void EndMiniGame()
    {
        if (EggGameManager.Instance.GetScore() >= minScoreToComplete)
        {
            Debug.Log("Mini-game completed successfully!");
            miniGameCanvas.SetActive(false);
            victoryPanel.SetActive(true);
            CancelInvoke(nameof(SpawnEgg));

        }
        else
        {
            Debug.Log("Try again! You need at least " + minScoreToComplete + " points.");
            EggGameManager.Instance.ResetState();
            failurePanel.SetActive(true);

        }
    }

    private void SpawnEgg()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(eggPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
    }
}
