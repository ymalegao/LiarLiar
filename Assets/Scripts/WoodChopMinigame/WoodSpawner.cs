using UnityEngine;
using TMPro;

public class WoodSpawner : MonoBehaviour
{
  public GameObject woodPrefab;
  public Transform[] spawnPoints;
  public static WoodSpawner Instance;

  public Transform axe;
  public float spawnRate = 2f;
  public GameObject miniGameCanvas;
  public GameObject victoryPanel;
  public GameObject instructionsPanel;

  public GameObject failurePanel;
  public TMP_Text instructionsText;
  public int minScoreToComplete = 10;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    miniGameCanvas.SetActive(false); // Ensure the mini-game starts hidden
    instructionsPanel.SetActive(true); // Show instructions first
    instructionsText.text = "Welcome to Wood Chopping!\n\nPress A and D to move the axe and chop wood.\n\nYou need a score of 10 to pass.\n\nPress Play to start!";
  }

  public void StartMiniGame()
  {
    instructionsPanel.SetActive(false); // Hide instructions
    miniGameCanvas.SetActive(true);
    failurePanel.SetActive(false);
    InvokeRepeating(nameof(SpawnWood), 1f, spawnRate);
    WoodGameManager.Instance.StartGame();
  }

  public void ExitGame()
  {
    WoodGameManager.Instance.ResetState();
    miniGameCanvas.SetActive(false);
    instructionsPanel.SetActive(false);
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
    WoodGameManager.Instance.EndGame();
    CancelInvoke(nameof(SpawnWood));
  }

  public void EndMiniGame()
  {
    if (WoodGameManager.Instance.GetScore() >= minScoreToComplete)
    {
      Debug.Log("Mini-game completed successfully!");
      miniGameCanvas.SetActive(false);
      victoryPanel.SetActive(true);
      CancelInvoke(nameof(SpawnWood));
    }
    else
    {
      Debug.Log("Try again! You need at least " + minScoreToComplete + " points.");
      WoodGameManager.Instance.ResetState();
      failurePanel.SetActive(true);
    }
  }

  private void SpawnWood()
  {
    int randomIndex = Random.Range(0, spawnPoints.Length);
    Instantiate(woodPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
  }
}