using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerHands : HandsBase
{
    // Fields

    public Transform[] hands;

    // Methods

    private void Update()
    {
        if (GameManager.gameOver == false)
        {
            AimHands();
            HandleUse();
            CheckPickUpOrDrop();
        }
    }

    private void AimHands()
    {
        // Create ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // Shoot ray and store hit info
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Go thru hands
            foreach (Transform hand in hands)
            {
                hand.LookAt(hitInfo.point); // Aim hand at the center of the screen
            }
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

    private void CheckPickUpOrDrop()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(CheckForButtonHold());
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

    // Coroutines

    private IEnumerator CheckForButtonHold()
    {
        float holdDuration = 0.5f;

        // Track & increase elapsed time
        for (float elapsedTime = 0f; elapsedTime < holdDuration; elapsedTime += Time.deltaTime)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                HandlePickup();

                yield break; // Break out of coroutine
            }

            yield return null; // Wait for next frame
        }

        HandleDrop();
    }
}
