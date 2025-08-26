using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
     public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

     public void LoadLevelOne()
    {
        SceneManager.LoadScene("LevelOne");
    }

    public void LoadHelpMenu()
    {
        SceneManager.LoadScene("HelpMenu");
    }

    public void LoadCreditsMenu()
    {
        SceneManager.LoadScene("CreditsMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); 
    }
}