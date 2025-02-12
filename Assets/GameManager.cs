using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro; // For UI display

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private float startingTime = 1000f;
    private NetworkVariable<float> countdownTimer = new NetworkVariable<float>(1000f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private TMP_Text timerText; // Assign in the inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Debug.Log("GameManager is ready!\n\n");
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Only the server should control the timer
        {
            Debug.Log("Server is ready to start the countdown!");
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        while (countdownTimer.Value > 0)
        {
            //Debug.Log($"Time remaining: {countdownTimer.Value} seconds");
            yield return new WaitForSeconds(1f);
            countdownTimer.Value -= 1f;
        }

        Debug.Log("Time's up!");
        EndGame();
    }

    private void Update()
    {
        if (timerText != null) // Update UI for all clients
        {
            int minutes = Mathf.FloorToInt(countdownTimer.Value / 60);
            int seconds = Mathf.FloorToInt(countdownTimer.Value % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over! Transitioning to End Game screen...");

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("End Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

}
