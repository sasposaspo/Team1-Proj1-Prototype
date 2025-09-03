using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Fields

    public static bool gameOver = false;

    private LevelTransitionController levelTransitionController;

    // Methods

    private void Start()
    {
        levelTransitionController = FindFirstObjectByType<LevelTransitionController>();
        gameOver = false;
    }

    // Button Methods

    public void LoadNextLevel()
    {
        StartCoroutine(levelTransitionController.LoadNextLevel());
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Ran Application.Quit");
    }

    // Coroutines

    public void PlayerDeath()
    {
        gameOver = true;
        StartCoroutine(levelTransitionController.ReloadLevel());
    }
}
