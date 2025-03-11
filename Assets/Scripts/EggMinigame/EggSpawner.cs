using UnityEngine;
using TMPro;

public class EggSpawner : MonoBehaviour
{
  public GameObject eggPrefab;

  private GetTask currentPlayer;

  public GameObject obstaclePrefab;
  public Transform[] spawnPoints;
  public static EggSpawner Instance;

  public Transform basket;
  public float spawnRate = 2f;
  public GameObject miniGameCanvas;
  public GameObject victoryPanel;
  public GameObject instructionsPanel;

  public int obstaclespawnRate = 5;

  public GameObject failurePanel;
  public TMP_Text instructionsText;
  public int minScoreToComplete = 3;

  private void Start()
  {
    miniGameCanvas.SetActive(false); // Ensure the mini-game starts hidden
    instructionsPanel.SetActive(true); // Show instructions first
  }

  private void Awake()
  {
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
    miniGameCanvas.SetActive(false);
    instructionsPanel.SetActive(false);
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
    EggGameManager.Instance.endGame();
    //trigger some condition if you want here using like a fakeNPC powerup Manger
    CancelInvoke(nameof(SpawnEgg));
    if (currentPlayer != null)
    {
      Debug.Log("Deactivating player camera");
      currentPlayer.ActivatePlayerCamera();
    }else{
      Debug.LogError("No player assigned to the game!");
    }
  }

  public void EndMiniGame()
  {
    CancelInvoke(nameof(SpawnEgg));
    miniGameCanvas.SetActive(false);
  }

  private void SpawnEgg()
  {
    int EggOrObstacle = Random.Range(0, 10);
    if (EggOrObstacle < 8)
    {
      int randomIndex = Random.Range(0, spawnPoints.Length);
      Instantiate(eggPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
    }
    else
    {
      int randomIndex = Random.Range(0, spawnPoints.Length);
      Vector3 spawnPosition = new Vector3(basket.position.x, spawnPoints[randomIndex].position.y, basket.position.z);
      Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }

  }

  public void SetPlayer(GetTask player)
  {
    currentPlayer = player;
  }

  private void SpawnObstacle()
  {
    int randomIndex = Random.Range(0, spawnPoints.Length);
    Instantiate(obstaclePrefab, spawnPoints[randomIndex].position, Quaternion.identity);
  }
}
