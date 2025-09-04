using UnityEngine;

public class GoToLevelTwo : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelTransitionController levelTransitionController = GameObject.FindFirstObjectByType<LevelTransitionController>();

            StartCoroutine(levelTransitionController.LoadNextLevel());
        }
    }
}
