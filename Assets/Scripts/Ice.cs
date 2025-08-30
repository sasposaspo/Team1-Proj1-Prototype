using UnityEngine;
using System.Collections;

public class Ice : MonoBehaviour, IMeltable
{
    // Fields

    private AnimationCurve smoothAnimationCurve;

    // Methods

    private void Start()
    {
        // Customize animation curve
        smoothAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        smoothAnimationCurve.postWrapMode = WrapMode.PingPong;
    }

    // Meltable Interface

    public void Melt()
    {
        StartCoroutine(MeltAnimation());
    }

    // Coroutines

    public IEnumerator MeltAnimation()
    {
        // Wait for ice to melt & destroy game object
        yield return ScaleAnimation(transform.localScale, Vector3.zero);
        Destroy(gameObject);
    }

    private IEnumerator ScaleAnimation(Vector3 startScale, Vector3 targetScale)
    {
        float duration = 3f;

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
