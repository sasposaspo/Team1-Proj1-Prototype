using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyHands : HandsBase
{
    // Fields

    public Transform[] hands;

    private bool canThrow = true;
    private bool lookingAtTarget = false;

    // Methods

    public void AimHands()
    {
        // Create ray from the center of the enemy to where the enemy is facing
        Ray ray = new Ray(transform.parent.root.position, transform.parent.root.forward);

        // Shoot ray and store hit info
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Go thru hands
            foreach (Transform hand in hands)
            {
                hand.LookAt(hitInfo.point); // Aim hand at the center of the screen
            }

            // Set flag depending on whether the enemy is looking at the player
            lookingAtTarget = hitInfo.collider.CompareTag("Player") ? true : false;
        }
    }

    public void HandleThrow()
    {
        if (canThrow == true && lookingAtTarget == true)
        {
            foreach (Transform hand in hands)
            {
                if (IsFull(hand) == true)
                {
                    Throw(hand);
                    StartCoroutine(ThrowCooldown());
                    return;
                }
            }
        }
    }

    public void HandlePickUp()
    {
        foreach (Transform hand in hands)
        {
            if (IsEmpty(hand) == true)
            {
                PickUp(hand);
                return;
            }
        }
    }

    // Coroutines

    private IEnumerator ThrowCooldown()
    {
        canThrow = false;
        yield return new WaitForSeconds(1f);
        canThrow = true;
    }

    // Return methods

    public bool EmptyHanded()
    {
        foreach (Transform hand in hands)
        {
            if (IsFull(hand) == true)
            {
                return false;
            }
        }

        return true;
    }

    public bool FullHanded()
    {
        foreach (Transform hand in hands)
        {
            if (IsEmpty(hand) == true)
            {
                return false;
            }
        }

        return true;
    }
}
