using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    // Fields

    private Vector3 normalScale;

    // Methods

    private void Start()
    {
        normalScale = transform.localScale;
        StartCoroutine(ScaleAnimation(Vector3.zero, normalScale));
    }

    // Meltable Interface

    public void Melt()
    {
        StopAllCoroutines();
        StartCoroutine(MeltAnimation());
    }

    // Collision Methods

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IHealth otherHealth))
        {
            otherHealth.Die();
        }
    }

    // Coroutines

    public IEnumerator MeltAnimation()
    {
        yield return ScaleAnimation(transform.localScale, Vector3.zero);
        Destroy(gameObject);
    }

    private IEnumerator ScaleAnimation(Vector3 startScale, Vector3 targetScale)
    {
        float duration = 1f;
        
        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, time);

            transform.localScale = currentScale; // Apply animation

            yield return null; // Wait for next frame
        }

        transform.localScale = targetScale; // Ensure finished animation state
    }
}
