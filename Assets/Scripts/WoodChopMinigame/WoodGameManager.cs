using UnityEngine;
using TMPro;

public class WoodGameManager : MonoBehaviour
{
  public static WoodGameManager Instance;

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
  public Transform axe;
  public float axeSpeed = 5f;

  private int score = 0;
  private bool gameActive = false;

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

    miniGameCanvas.SetActive(false); // Hide the mini-game initially
    instructionsPanel.SetActive(true); // Show instructions first
    failurePanel.SetActive(false);
    victoryPanel.SetActive(false);
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

    // Move Axe with Arrow Keys
    float moveInput = Input.GetAxis("Horizontal");
    axe.position += Vector3.right * moveInput * axeSpeed * Time.deltaTime;

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

  private void UpdateUI()
  {
    scoreText.text = "Score: " + score;
    timerText.text = "Time: " + Mathf.CeilToInt(timer);
  }

  public void AddScore()
  {
    score++;
    UpdateUI();
  }

  public void SubtractScore()
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
    WoodSpawner.Instance.EndMiniGame();
    overallGameCanvas.SetActive(false);

  }

  public void ResetState()
  {
    score = 0;
    timer = gameDuration;
    UpdateUI();
  }
}