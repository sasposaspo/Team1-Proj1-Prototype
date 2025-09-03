using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Snowball : HealthBase
{
    // Fields

    private Vector3 normalScale;
    private Coroutine scaleAnimation = null;

    // Methods

    private void Start()
    {
        normalScale = transform.localScale;
        StartCoroutine(ScaleAnimation(Vector3.zero, normalScale));
    }

    public override void Die()
    {
        if (scaleAnimation != null) { return; } // Do nothing if animation is running

        StartCoroutine(DeathAnimation());
    }

    // Coroutines

    private IEnumerator DeathAnimation()
    {
        // Wait for scaling animation to finish
        yield return scaleAnimation = StartCoroutine(ScaleAnimation(transform.localScale, Vector3.zero));

        Destroy(gameObject);
    }

    private IEnumerator ScaleAnimation(Vector3 startScale, Vector3 targetScale)
    {
        float duration = 0.5f;

        // Customize animation curve
        AnimationCurve smoothAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        smoothAnimationCurve.postWrapMode = WrapMode.PingPong;
        
        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, smoothAnimationCurve.Evaluate(time));

            transform.localScale = currentScale; // Apply animation

            yield return null; // Wait for next frame
        }

        transform.localScale = targetScale; // Ensure finished animation state
    }

    // Collision Methods

    private void OnCollisionStay(Collision collision)
    {
        // Check if collided object has health component
        if (collision.gameObject.TryGetComponent(out HealthBase otherHealth))
        {
            otherHealth.Die();
        }
    }
}
