using UnityEngine;
using TMPro;

public class WoodGameManager : MonoBehaviour, MinigameManager
{
    public GameObject GameCanvas { get; set; }
    public static WoodGameManager Instance;

    [Header("Game Settings")]
    public float gameDuration = 30f;
    private float timer;

    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject failurePanel;
    public GameObject victoryPanel;
    public GameObject instructionsPanel;

    [Header("Gameplay Elements")]
    public Transform axe;
    public float axeSpeed = 5f;

    private int score = 0;
    public bool gameActive = false;

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

        GameCanvas = GameObject.Find("WoodGameCanvas");
        if (GameCanvas != null)
        {
            GameCanvas.SetActive(false); // Hide the mini-game initially
        }
        instructionsPanel.SetActive(true); // Show instructions first
        failurePanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        StartGame();
    }

    private void OnDisable()
    {
        EndGame();
    }

    public void StartGame()
    {
        timer = gameDuration;
        score = 0;
        gameActive = true;
        if (GameCanvas != null)
        {
            GameCanvas.SetActive(true);
        }
        instructionsPanel.SetActive(false);
        failurePanel.SetActive(false);
        victoryPanel.SetActive(false);
        UpdateUI();
    }

    public void EndGame()
    {
        gameActive = false;
        if (GameCanvas != null)
        {
            GameCanvas.SetActive(false);
        }
        WoodSpawner.Instance.EndMiniGame();
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

    public void SubtractScore()
    {
        score--;
        UpdateUI();
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetState()
    {
        score = 0;
        timer = gameDuration;
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
}