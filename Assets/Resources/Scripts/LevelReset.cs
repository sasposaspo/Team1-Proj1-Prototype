using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelReset : MonoBehaviour
{
    void Update()
    {
        // Check if the player presses the R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload the currently active scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}