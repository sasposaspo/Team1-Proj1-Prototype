using System.Collections;
using UnityEngine;

public class Snowball : HealthBase
{
    // Fields

    Vector3 normalScale;

    // Methods

    private void Start()
    {
        normalScale = transform.localScale;

        StartCoroutine(SpawnAnimation());
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    // Collision Methods

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out HealthBase health))
        {
            health.ModifyHealth(-1);
        }
    }

    // Coroutines

    private IEnumerator SpawnAnimation()
    {
        float duration = 1f;
        
        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            Vector3 currentScale = Vector3.Lerp(Vector3.zero, normalScale, time);

            transform.localScale = currentScale; // Apply animation

            yield return null; // Wait for next frame
        }

        transform.localScale = normalScale; // Ensure finished animation state
    }
}
