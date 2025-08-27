using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour
{
    // Fields

    public float hoverPositionOffset = 0.15f;

    private float hoverMinY;
    private float hoverMaxY;
    
    private AnimationCurve smoothAnimationCurve;

    // Methods

    private void Awake()
    {
        // Get the lower & upper positions for the hover animation
        hoverMinY = transform.localPosition.y - hoverPositionOffset;
        hoverMaxY = transform.localPosition.y + hoverPositionOffset;

        // Customize animation curve
        smoothAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        smoothAnimationCurve.postWrapMode = WrapMode.PingPong;
    }

    private void OnEnable()
    {
        StartCoroutine(HoverAnimationLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Coroutines

    private IEnumerator HoverAnimationLoop()
    {
        bool isMovingDownward = true;

        while (true) // Runs forever unless stopped
        {
            // Toggle state
            isMovingDownward = !isMovingDownward;

            // Get target position based on state
            float targetY = isMovingDownward ? hoverMinY : hoverMaxY;

            // Start animation and wait for it to finish
            yield return StartCoroutine(HoverAnimation(targetY));
        }
    }

    private IEnumerator HoverAnimation(float targetY)
    {
        float duration = 1f;
        float startY = transform.localPosition.y;

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            float currentY = Mathf.Lerp(startY, targetY, smoothAnimationCurve.Evaluate(time));

            transform.localPosition = new Vector3(transform.localPosition.x, currentY, transform.localPosition.z); // Apply animation

            yield return null; // Wait for next frame
        }

        transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z); // Ensure finished animation state
    }
}
