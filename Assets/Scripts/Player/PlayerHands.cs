using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerHands : HandsBase
{
    // Fields

    public Transform[] hands;

    private float holdThreshold = 0.5f;
    private float doublePressWindow = 0.25f;

    private int pressCount = 0;
    private Coroutine interactionRoutine = null;

    // Methods

    private void Update()
    {
        if (GameManager.gameOver == false)
        {
            AimHands();
            HandleUse();
            CheckForPickUpOrDropOrSwap();
        }
    }

    private void AimHands()
    {
        // Angular speed in radians per second
        float speed = 2f;

        // Go thru hands
        foreach (Transform hand in hands)
        {
            // Determine which direction to rotate towards
            Vector3 targetDirection = Target() - hand.position;

            // The step size is equal to speed times frame time
            float singleStep = speed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(hand.forward, targetDirection, singleStep, 0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(hand.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            hand.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void HandleUse()
    {
        // Go thru hands
        for (int i = 0; i < hands.Length; i++)
        {
            // Use hand that corresponds to mouse button
            if (Input.GetMouseButtonDown(i)) { Use(hands[i]); }
        }
    }

    private void CheckForPickUpOrDropOrSwap()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            pressCount++;

            if (interactionRoutine == null)
            {
                interactionRoutine = StartCoroutine(DetectIntent());
            }
        }
    }

    private void HandlePickup()
    {
        // Pickup with right hand first
        foreach (Transform hand in hands.Reverse().ToArray())
        {
            if (IsEmpty(hand) == true)
            {
                PickUp(hand);
                return;
            }
        }
    }

    private void HandleDrop()
    {
        // Drop with left hand first
        foreach (Transform hand in hands)
        {
            if (IsFull(hand) == true)
            {
                Drop(hand);
                return;
            }
        }
    }

    private void HandleSwap()
    {
        // Swap array elements
        (hands[0], hands[1]) = (hands[1], hands[0]);

        // Store positions
        Vector3 leftHandPosition = LeftHand().localPosition;
        Vector3 rightHandPosition = RightHand().localPosition;

        // Start swap animations
        StartCoroutine(SwapAnimation(LeftHand(), rightHandPosition));
        StartCoroutine(SwapAnimation(RightHand(), leftHandPosition));
    }

    private void ResetInteraction()
    {
        pressCount = 0;
        interactionRoutine = null;
    }

    // Coroutines

    private IEnumerator DetectIntent()
    {
        float elapsedTime = 0f;

        // Waif for potential second press or hold
        while (elapsedTime < holdThreshold)
        {
            // Increase elapsed time
            elapsedTime += Time.deltaTime;

            // If key released early and only one press
            if (Input.GetKeyUp(KeyCode.E) && pressCount == 1 && elapsedTime < doublePressWindow)
            {
                yield return new WaitForSeconds(doublePressWindow - elapsedTime);

                if (pressCount == 1)
                {
                    HandlePickup();
                    ResetInteraction();
                    yield break;
                }
            }

            // If second press detected within window
            if (pressCount >= 2 && elapsedTime < doublePressWindow)
            {
                HandleSwap();
                ResetInteraction();
                yield break;
            }

            yield return null; // Wait for next frame
        }

        // If key still held beyond hold threshold
        if (Input.GetKey(KeyCode.E))
        {
            HandleDrop();
        }
        else
        {
            HandlePickup(); // Fallback if released after threshold but not held
        }

        ResetInteraction();
    }

    private IEnumerator SwapAnimation(Transform hand, Vector3 targetPosition)
    {
        // Customize animation curve
        AnimationCurve smoothAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        smoothAnimationCurve.postWrapMode = WrapMode.PingPong;

        float duration = 0.25f;
        Vector3 startPosition = hand.localPosition;

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, smoothAnimationCurve.Evaluate(time));

            hand.localPosition = currentPosition; // Apply animation

            yield return null; // Wait for next frame
        }

        hand.localPosition = targetPosition; // Ensure finished animation state
    }

    // Return Methods

    private Transform LeftHand()
    {
        return hands[0];
    }

    private Transform RightHand()
    {
        return hands[1];
    }

    private Vector3 Target()
    {
        // Create ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // Shoot ray and store hit info
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }

        // Fallback: return arbitrary distance forward
        return ray.origin + ray.direction * 100f;
    }
}
