using UnityEngine;
using TMPro;
using System.Collections;
using PowerUps;
using System; // Add this to access PowerUpManager

public class EggGameManager : MonoBehaviour, MinigameManager
{
  public GameObject GameCanvas { get; set; }

  public static EggGameManager Instance;

  [Header("Game Settings")]
  public float gameDuration = 30f;
  private float timer;
  public int winningScore = 3; // Score needed to win and unlock power-up

  [Header("UI Elements")]
  public TMP_Text scoreText;
  public TMP_Text timerText;
  public GameObject miniGameCanvas;
  public GameObject failurePanel;
  public GameObject victoryPanel;
  public GameObject instructionsPanel;
  public GameObject powerUpUnlockedText; // Add a UI element to show power-up unlock

  public GameObject overallGameCanvas;

  public GameObject seekerSelectButton;
  public GameObject helpButton;


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
    seekerSelectButton.SetActive(false);
    helpButton.SetActive(false);
    if (powerUpUnlockedText != null)
      powerUpUnlockedText.SetActive(false);
    Debug.Log("EggGameManager Awake");
  }

  public void endGame()
  {
    overallGameCanvas.SetActive(false);
    seekerSelectButton.SetActive(true);
    helpButton.SetActive(true);

  }

  public void StartGame()
  {
    timer = gameDuration;
    score = 0;
    gameActive = true;
    powerUpUnlockedLocally = false; // Reset power-up status on game start

    // Make sure power-up is disabled at start of game
    if (PowerUpManager.Instance != null)
    {
      PowerUpManager.Instance.DisablePowerUp();
    }

    miniGameCanvas.SetActive(true);
    instructionsPanel.SetActive(false);
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
    seekerSelectButton.SetActive(false);
    helpButton.SetActive(false);
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

      // Check if win condition is met
      if (score >= winningScore && !powerUpUnlockedLocally)
      {
        UnlockPowerUp();
      }
    }
    else
    {
      EndGame();
    }

    // Allow using the powerup with F key only if unlocked
    // We check both our local flag AND the network state via PowerUpManager
    if (powerUpUnlockedLocally &&
        PowerUpManager.Instance != null &&
        PowerUpManager.Instance.IsPowerUpAvailable() &&
        Input.GetKeyDown(KeyCode.F))
    {
      UsePowerUp();
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

    // Check if win condition is met
    if (score >= winningScore && !powerUpUnlockedLocally)
    {
      UnlockPowerUp();
    }
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

  private void UnlockPowerUp()
  {
    powerUpUnlockedLocally = true;
    Debug.Log("Power Up Unlocked! Press F to use it.");

    // Tell PowerUpManager that the power-up is available
    if (PowerUpManager.Instance != null)
    {
      PowerUpManager.Instance.EnablePowerUp();
    }
    else
    {
      Debug.LogError("PowerUpManager Instance is null!");
    }

    // Show power-up unlocked message
    if (powerUpUnlockedText != null)
    {
      powerUpUnlockedText.SetActive(true);
      powerUpUnlockedText.GetComponent<TMP_Text>().text = "Fog Power-Up Unlocked! Press F to use it.";
      // Optional: Hide the message after a few seconds
      StartCoroutine(HidePowerUpMessageAfterDelay(5f));
    }
  }

  private IEnumerator HidePowerUpMessageAfterDelay(float delay)
  {
    yield return new WaitForSeconds(delay);
    if (powerUpUnlockedText != null)
      powerUpUnlockedText.SetActive(false);
  }

  private void UsePowerUp()
  {
    if (PowerUpManager.Instance != null)
    {
      Debug.Log("Using fog power-up!");
      PowerUpManager.Instance.RequestApplyVisionReductionServerRpc();
      powerUpUnlockedLocally = false; // One-time use locally

      // PowerUpManager will handle disabling it on the network after use

      // Optional: Show a usage feedback
      if (powerUpUnlockedText != null)
      {
        powerUpUnlockedText.SetActive(true);
        powerUpUnlockedText.GetComponent<TMP_Text>().text = "Fog Applied!";
        StartCoroutine(HidePowerUpMessageAfterDelay(2f));
      }
    }
    else
    {
      Debug.LogError("PowerUpManager Instance is null!");
    }
  }

  public void EndGame()
  {
    gameActive = false;
    miniGameCanvas.SetActive(false);

    // Show appropriate end game panel
    if (score >= winningScore)
    {   
      failurePanel.SetActive(false);
      victoryPanel.SetActive(true);
    }
    else
    {
      victoryPanel.SetActive(false);
      failurePanel.SetActive(true);
      ResetState();
    }


    EggSpawner.Instance.EndMiniGame();
  }

  public void ResetState()
  {
    score = 0;
    timer = gameDuration;
    powerUpUnlockedLocally = false;

    // Make sure power-up is disabled when resetting
    if (PowerUpManager.Instance != null)
    {
      PowerUpManager.Instance.DisablePowerUp();
    }

    UpdateUI();
    if (powerUpUnlockedText != null)
      powerUpUnlockedText.SetActive(false);
  }
}