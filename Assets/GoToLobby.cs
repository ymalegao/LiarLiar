using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class goToScene : MonoBehaviour
{
    public void goToLobby()
    {
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient))
        {
            Debug.Log("Shutting down Netcode before returning to the Lobby...");
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("Lobby"); 
    }

    public void goToTitle()
    {
        SceneManager.LoadScene("Title Screen"); 
    }

    public void goToHowTo()
    {
        SceneManager.LoadScene("How To Play"); 
    }

    public void goToCredits()
    {
        SceneManager.LoadScene("Credits"); 
    }

    public void goToEndGame()
    {
        SceneManager.LoadScene("End Game"); 
    }
}
