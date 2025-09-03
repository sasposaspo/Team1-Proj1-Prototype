using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTransitionController : MonoBehaviour
{
    // Fields

    private CanvasGroup canvasGroup;

    // Methods

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        StartCoroutine(FadeFromBlack());
    }

    // Level Transition Coroutines

    public IEnumerator ReloadLevel()
    {
        yield return FadeToBlack();

        int currentScene = SceneManager.GetActiveScene().buildIndex; // Get index of current scene
        SceneManager.LoadScene(currentScene);
    }

    public IEnumerator LoadNextLevel()
    {
        yield return FadeToBlack();

        int currentScene = SceneManager.GetActiveScene().buildIndex; // Get index of current scene
        SceneManager.LoadScene(currentScene + 1);
    }

    public IEnumerator LoadMainMenu()
    {
        yield return FadeToBlack();
        // TODO Load main menu
    }

    // Helper Animation Coroutines

    public IEnumerator FadeToBlack()
    {
        yield return FadeAnimation(0f, 1f);
    }

    public IEnumerator FadeFromBlack()
    {
        yield return FadeAnimation(1f, 0f);
    }

    // Animation Coroutines

    private IEnumerator FadeAnimation(float startAlpha, float targetAlpha)
    {
        float duration = 0.25f;

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsedTime

            // Interpolate over time
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, time);

            canvasGroup.alpha = currentAlpha; // Apply animation

            yield return null; // Wait for next frame
        }

        canvasGroup.alpha = targetAlpha; // Ensure finished animation state
    }
}
