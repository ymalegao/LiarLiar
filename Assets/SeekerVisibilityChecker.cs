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
    StartCoroutine(WaitForSeekerRole());

  }

  private IEnumerator WaitForSeekerRole()
  {
    while (ServerManager.Instance == null || !ServerManager.Instance.IsSpawned)
    {
      yield return new WaitForSeconds(0.5f);
    }

    ulong myClientId = NetworkManager.Singleton.LocalClientId;
    seekerSpecificUI.SetActive(true);
    // if (ServerManager.Instance.seekerClientId.Value == myClientId)
    // {
    //   seekerSpecificUI.SetActive(true);
    // }
    // else
    // {
    //   gameObject.SetActive(false); // Hide UI
    // }

  }
}
