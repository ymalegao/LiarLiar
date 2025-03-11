using UnityEngine;
using TMPro;
using System.Collections;
using PowerUps;
using UnityEngine.UI;

public class EggGameManager : MonoBehaviour, MinigameManager
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
  private bool powerUpUsed = false; // Track if the power-up has been used

  [Header("UI Elements")]
  // ... existing UI elements ...
  public Button usePowerUpButton; // Add this line

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
    powerUpUsed = false; // Reset power-up used flag
    if (usePowerUpButton != null)
    {
      usePowerUpButton.onClick.RemoveAllListeners();
      usePowerUpButton.gameObject.SetActive(false);
      usePowerUpButton.interactable = true;
    }

    miniGameCanvas.SetActive(true);
    instructionsPanel.SetActive(false);
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
    if (powerUpUnlockedText != null)
      powerUpUnlockedText.SetActive(false);
    UpdateUI();
  }

  // Modified to check both local and network power-up status

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

    // Note: F key is now handled in PowerUpManager.Update()
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
    powerUpUsed = false; // Make sure it's marked as not used
    Debug.Log("Power Up Unlocked! Press F to use it.");

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

  // Called by PowerUpManager when power-up is used (via ClientRpc)
  public void PowerUpUsed()
  {
    Debug.Log("PowerUpUsed called in EggGameManager");
    powerUpUsed = true;

    // Update UI if needed
    if (powerUpUnlockedText != null)
    {
      powerUpUnlockedText.SetActive(true);
      powerUpUnlockedText.GetComponent<TMP_Text>().text = "Fog Applied!";
      StartCoroutine(HidePowerUpMessageAfterDelay(2f));
    }
  }

  public void EndGame()
  {
    gameActive = false;
    miniGameCanvas.SetActive(false);

    // Show appropriate end game panel
    if (score >= winningScore)
    {
      victoryPanel.SetActive(true);

      // Enable the power-up button if the player won
      if (usePowerUpButton != null)
      {
        usePowerUpButton.gameObject.SetActive(true);
        usePowerUpButton.onClick.AddListener(UsePowerUp);
      }
    }
    else
    {
      failurePanel.SetActive(true);

      // Make sure the button is disabled if player lost
      if (usePowerUpButton != null)
      {
        usePowerUpButton.gameObject.SetActive(false);
      }
    }

    EggSpawner.Instance.EndMiniGame();
  }

  public void ResetState()
  {
    score = 0;
    timer = gameDuration;
    powerUpUnlockedLocally = false;
    powerUpUsed = false;

    if (usePowerUpButton != null)
    {
      usePowerUpButton.onClick.RemoveAllListeners();
      usePowerUpButton.gameObject.SetActive(false);
      usePowerUpButton.interactable = true;
    }


    UpdateUI();
    if (powerUpUnlockedText != null)
      powerUpUnlockedText.SetActive(false);
  }

  public void UsePowerUp()
  {
    // Only allow using the power-up once
    if (!powerUpUsed && PowerUps.PowerUpManager.Instance != null)
    {
      Debug.Log("Using power-up from victory screen");
      // Call the ServerRpc to apply the vision effect
      PowerUps.PowerUpManager.Instance.RequestApplyVisionReductionServerRpc();
      powerUpUsed = true;

      // Disable the button after use
      if (usePowerUpButton != null)
      {
        usePowerUpButton.interactable = false;
      }

      // Show a message that the power-up was used
      if (powerUpUnlockedText != null)
      {
        powerUpUnlockedText.SetActive(true);
        powerUpUnlockedText.GetComponent<TMP_Text>().text = "Fog Power-Up Used!";
        StartCoroutine(HidePowerUpMessageAfterDelay(2f));
      }
    }
  }
}
