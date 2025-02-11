using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class EndGameUI : MonoBehaviour
{
    public void ReturnToLobby()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            LobbyManager.Instance.ReturnToLobby();
        }
    }
}