using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    // Fields

    private Rigidbody rb;
    private Collider col;

    private Coroutine pickupAnimation;

    private AnimationCurve smoothAnimationCurve;

    public Transform playerHand;

    // Methods

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Customize animation curve
        smoothAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        smoothAnimationCurve.postWrapMode = WrapMode.PingPong;
    }

    public virtual void PickUp(Transform hand)
    {
        TogglePhysics(false);
        transform.SetParent(hand);
        col.enabled = false;
        rb.isKinematic = true;

        GetComponentInChildren<Rotation>().enabled = false;
        GetComponentInChildren<Hover>().enabled = false;

        if (pickupAnimation != null)
        {
            StopCoroutine(pickupAnimation);
        }

        pickupAnimation = StartCoroutine(PickupAnimation());
    }

    public virtual void Drop()
    {
        if (pickupAnimation != null)
        {
            StopCoroutine(pickupAnimation);
        }

        transform.rotation = Quaternion.Euler(Vector3.zero);

        transform.SetParent(null);
        col.enabled = true;
        rb.isKinematic = false;
        TogglePhysics(true);

        GetComponentInChildren<Rotation>().enabled = true;
        GetComponentInChildren<Hover>().enabled = true;
    }

    public virtual void Use()
    {
        if (CanUse() == false)
        {
            StartCoroutine(ShakeAnimationLoop());
        }
    }

    protected virtual void TogglePhysics(bool enable)
    {
        rb.isKinematic = !enable; // When isKinematic is disabled, the rigidbody will respond to physics

        // Set rigidbody to interpolate when responding to physics
        rb.interpolation = enable ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
    }

    // Coroutines

    private IEnumerator PickupAnimation()
    {
        float duration = 0.15f;

        transform.GetLocalPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation); // Store start position & start rotation

        Vector3 targetPosition = Vector3.zero;
        Quaternion targetRotation = Quaternion.Euler(Vector3.zero);

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate position & rotation over time
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, time);
            Quaternion currentRotation = Quaternion.Lerp(startRotation, targetRotation, time);

            transform.SetLocalPositionAndRotation(currentPosition, currentRotation); // Apply interpolations to transform

            yield return null; // Wait for next frame
        }

        transform.SetLocalPositionAndRotation(targetPosition, targetRotation); // Ensure target position & target rotation are set
    }

    // Coroutines

    private IEnumerator ShakeAnimationLoop()
    {
        float angleOffset = 15f;

        for (int i = 0; i < 5; i++)
        {
            float targetAngle = (i % 2 == 0) ? angleOffset : -angleOffset;
            yield return ShakeAnimation(targetAngle);
        }

        yield return ShakeAnimation(0f);
    }

    private IEnumerator ShakeAnimation(float targetZ)
    {
        float duration = 0.05f;

        // Normalize angles to handle wraparound
        float startZ = NormalizeAngle(transform.localRotation.z);
        targetZ = NormalizeAngle(targetZ);

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            // Normalize elapsed time
            float time = elapsedTime / duration;

            // Interpolate over time
            float currentZ = Mathf.Lerp(startZ, targetZ, smoothAnimationCurve.Evaluate(time));

            // Apply animation
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, currentZ);

            yield return null; // Wait for next frame
        }

        // Ensure finished animation state
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, targetZ);
    }

    // Return Methods

    protected virtual bool CanUse(Func<bool> condition = null)
    {
        if (condition == null || condition() == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private float NormalizeAngle(float angle)
    {
        // Ensure angle falls within range of -180 to 180 degrees
        return (angle > 180f) ? angle - 360f : angle;
    }
}
