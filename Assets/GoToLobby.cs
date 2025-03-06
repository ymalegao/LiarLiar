using UnityEngine;
using UnityEngine.SceneManagement;

public class goToScene : MonoBehaviour
{
    public void goToLobby()
    {
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