using UnityEngine;
using TMPro;
using System.Collections;

public class EggGameManager : MonoBehaviour
{
  public GameObject GameCanvas { get; set; }

  public static EggGameManager Instance;

  [Header("Game Settings")]
  public float gameDuration = 30f;
  private float timer;
  public int winningScore = 2; // Score needed to win and unlock power-up

  [Header("UI Elements")]
  public TMP_Text scoreText;
  public TMP_Text timerText;
  public GameObject miniGameCanvas;
  public GameObject failurePanel;
  public GameObject victoryPanel;
  public GameObject instructionsPanel;
  public GameObject powerUpUnlockedText; // Add a UI element to show power-up unlock

  public GameObject overallGameCanvas;

  [Header("Gameplay Elements")]
  public Transform basket;
  public float basketSpeed = 5f;

  private int score = 0;
  public bool gameActive = false;
  private bool powerUpUnlockedLocally = false; // Track locally if the power-up has been unlocked

  private void Awake()
  {
    Instance = this;
    miniGameCanvas.SetActive(false); // Hide the mini-game initially
    instructionsPanel.SetActive(true); // Show instructions first
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
    if (powerUpUnlockedText != null)
      powerUpUnlockedText.SetActive(false);
    Debug.Log("EggGameManager Awake");
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
    powerUpUnlockedLocally = false; // Reset power-up status on game start


    miniGameCanvas.SetActive(true);
    instructionsPanel.SetActive(false);
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
    if (powerUpUnlockedText != null)
      powerUpUnlockedText.SetActive(false);
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


  private IEnumerator HidePowerUpMessageAfterDelay(float delay)
  {
    yield return new WaitForSeconds(delay);
    if (powerUpUnlockedText != null)
      powerUpUnlockedText.SetActive(false);
  }


  public void EndGame()
  {
    gameActive = false;
    miniGameCanvas.SetActive(false);

    // Show appropriate end game panel
    if (score >= winningScore)
    {
      victoryPanel.SetActive(true);
    }
    else
    {
      failurePanel.SetActive(true);
    }

    EggSpawner.Instance.EndMiniGame();
  }
}