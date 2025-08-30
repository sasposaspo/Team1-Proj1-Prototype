using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    // Fields

    public Transform platform;
    public Transform[] points;

    public float moveDuration = 3f;

    private Rigidbody rb;

    private AnimationCurve smoothAnimationCurve;

    // Methods

    private void OnEnable()
    {
        // Customize animation curve
        smoothAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        smoothAnimationCurve.postWrapMode = WrapMode.PingPong;

        rb = platform.GetComponent<Rigidbody>();

        StartCoroutine(Move());
    }

    // Coroutines

    private IEnumerator Move()
    {
        while (true) // Runs forever unless stopped
        {
            // Go thru points list
            foreach (Transform point in points)
            {
                // Move platform & wait 1 second
                yield return MoveAnimation(point.position, moveDuration);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator MoveAnimation(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = platform.position;

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, smoothAnimationCurve.Evaluate(time));

            rb.MovePosition(currentPosition); // Apply animation

            yield return null; // Wait for next frame
        }

        rb.MovePosition(targetPosition); // Ensure finished animation state
    }
}
