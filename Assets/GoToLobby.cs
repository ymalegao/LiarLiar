using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class goToScene : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;
    public void goToLobby()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Host is returning to the lobby...");

            // Notify all clients to return to the lobby
            LoadLobbyClientRpc();

            // Host loads the lobby scene
            PlayButtonSoundAndLoadScene("Lobby");
        }
        else if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Client is waiting for host to return to the lobby...");
        }
        else
        {
            // If not connected, just load the lobby scene
            PlayButtonSoundAndLoadScene("Lobby");
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
        PlayButtonSoundAndLoadScene("Title Screen");
    }

    public void goToHowTo()
    {
        PlayButtonSoundAndLoadScene("How To Play");
    }

    public void goToCredits()
    {
        PlayButtonSoundAndLoadScene("Credits");
    }

    public void goToEndGame()
    {
        PlayButtonSoundAndLoadScene("End Game");
    }

    private void PlayButtonSoundAndLoadScene(string sceneName)
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
            StartCoroutine(LoadSceneAfterDelay(sceneName, buttonClickSound.length));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

}
