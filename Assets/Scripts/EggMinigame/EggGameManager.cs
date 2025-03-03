using UnityEngine;
using TMPro;
using System.Collections;

public class EggGameManager : MonoBehaviour, MinigameManager
{

  public GameObject GameCanvas { get; set; }

  public static EggGameManager Instance;

  [Header("Game Settings")]
  public float gameDuration = 30f;
  private float timer;

  [Header("UI Elements")]
  public TMP_Text scoreText;
  public TMP_Text timerText;
  public GameObject miniGameCanvas;
  public GameObject failurePanel;
  public GameObject victoryPanel;
  public GameObject instructionsPanel;

  public GameObject overallGameCanvas;

  [Header("Gameplay Elements")]
  public Transform basket;
  public float basketSpeed = 5f;

  private int score = 0;
  public bool gameActive = false;


  private void Awake()
  {
    Instance = this;
    miniGameCanvas.SetActive(false); // Hide the mini-game initially
    instructionsPanel.SetActive(true); // Show instructions first
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
  }

  public void endGame()
  {
    overallGameCanvas.SetActive(false);
  }

  public void StartGame()
  {
    timer = gameDuration;
    score = 0;
    gameActive = true;
    miniGameCanvas.SetActive(true);
    instructionsPanel.SetActive(false);
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
    UpdateUI();
  }

  private void Update()
  {
    if (!gameActive) return;

    // Move Basket with Arrow Keys
    float moveInput = Input.GetAxis("Horizontal");
    basket.position += Vector3.right * moveInput * basketSpeed * Time.deltaTime;

    // Update Timer
    if (timer > 0)
    {
      timer -= Time.deltaTime;
      UpdateUI();
    }
    else
    {
      EndGame();
    }
  }

  public void UpdateUI()
  {
    scoreText.text = "Score: " + score;
    timerText.text = "Time: " + Mathf.CeilToInt(timer);
  }

  public void AddScore()
  {
    score++;
    UpdateUI();
  }

  public void subtractScore()
  {
    score--;
    UpdateUI();
  }



  public int GetScore()
  {
    return score;
  }

  public void EndGame()
  {
    gameActive = false;
    miniGameCanvas.SetActive(false);
    EggSpawner.Instance.EndMiniGame();
    this.gameObject.SetActive(false);
  }

  public void ResetState()
  {
    score = 0;
    timer = gameDuration;
    UpdateUI();

  }
}
