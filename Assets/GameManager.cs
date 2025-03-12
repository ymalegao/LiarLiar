using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro; // For UI display

public class GameManager : NetworkBehaviour
{
  public static GameManager Instance;

  [SerializeField] private float startingTime = 1500f;
  private NetworkVariable<float> countdownTimer = new NetworkVariable<float>(1500f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

  [SerializeField] private TMP_Text timerText; // Assign in the inspector

  private void Awake()
  {
    if (Instance == null) Instance = this;
  }

  public override void OnNetworkSpawn()
  {
    if (IsServer) // Only the server should control the timer
    {
      StartCoroutine(StartCountdown());
    }
  }

  private IEnumerator StartCountdown()
  {
    while (countdownTimer.Value > 0)
    {
      yield return new WaitForSeconds(1f);
      countdownTimer.Value -= 1f;
    }

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

    if (NetworkManager.Singleton.IsServer)
    {
      NetworkManager.Singleton.SceneManager.LoadScene("End Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
  }

}
