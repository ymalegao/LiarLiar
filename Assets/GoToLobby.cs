using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void goToLobby()
    {
        SceneManager.LoadScene("Lobby"); // Replace with your lobby scene's name
    }

    public void goToTitle()
    {
        SceneManager.LoadScene("Title Screen"); // Replace with your lobby scene's name
    }

    public void goToHowTo()
    {
        SceneManager.LoadScene("How To Play"); // Replace with your lobby scene's name
    }

    public void goToCredits()
    {
        SceneManager.LoadScene("Credits"); // Replace with your lobby scene's name
    }
}