using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{

  [SerializeField] private Button ServerBtn;
  [SerializeField] private Button ClientBtn;
  [SerializeField] private Button HostBtn;

  private void Awake()
  {
    ServerBtn.onClick.AddListener(() =>
    {
      NetworkManager.Singleton.StartServer();
    });
    ClientBtn.onClick.AddListener(() =>
    {
      NetworkManager.Singleton.StartClient();
    });
    HostBtn.onClick.AddListener(() =>
    {
      NetworkManager.Singleton.StartHost();
    });
  }
}
