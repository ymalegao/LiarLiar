using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class goToScene : NetworkBehaviour
{
    public void goToLobby()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Host is returning to the lobby...");

            // Notify all clients to return to the lobby
            LoadLobbyClientRpc();

            // Host loads the lobby scene
            SceneManager.LoadScene("Lobby");
        }
        else if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Client is waiting for host to return to the lobby...");
        }
        else
        {
            // If not connected, just load the lobby scene
            SceneManager.LoadScene("Lobby");
        }
    }

    [ClientRpc]
    private void LoadLobbyClientRpc()
    {
        Debug.Log("Client received instruction to return to the lobby...");

        // Shutdown the network manager (if connected)
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Load the lobby scene
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
