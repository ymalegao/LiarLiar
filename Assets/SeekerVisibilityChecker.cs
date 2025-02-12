using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class SeekerVisibilityChecker : MonoBehaviour
{
  // Start is called before the first frame update
  [SerializeField] private GameObject seekerSpecificUI;
  void Start()
  {
    Debug.Log("SeekerSelectionUI Start");
    StartCoroutine(WaitForSeekerRole());

  }

  private IEnumerator WaitForSeekerRole()
  {
    Debug.Log("Waiting for ServerManager to spawn...");
    while (ServerManager.Instance == null || !ServerManager.Instance.IsSpawned)
    {
      Debug.Log("ServerManager not spawned yet...");
      yield return new WaitForSeconds(0.5f);
    }

    Debug.Log("ServerManager spawned. Checking if this client is the Seeker...");
    ulong myClientId = NetworkManager.Singleton.LocalClientId;
    Debug.Log($"My client ID: {myClientId}");
    Debug.Log($"Seeker client ID: {ServerManager.Instance.seekerClientId.Value}");
    if (ServerManager.Instance.seekerClientId.Value == myClientId)
    {
      Debug.Log("✅ This client is the Seeker.");
      seekerSpecificUI.SetActive(true);
    }
    else
    {
      Debug.Log("🚫 This client is NOT the Seeker.");
      gameObject.SetActive(false); // Hide UI
    }

  }
}
