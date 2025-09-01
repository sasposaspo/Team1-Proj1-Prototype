using UnityEngine;
using System.Linq;

public abstract class HandsBase : MonoBehaviour
{
    // Methods

    public void PickUp(Transform hand)
    {
        // Get reference to nearest pickup
        if (GetNearestPickup(out PickupBase nearestPickup))
        {
            nearestPickup.PickUp(hand);
        }
    }

    public void Drop(Transform hand)
    {
        // Get reference to held pickup
        if (GetHeldPickup(hand, out PickupBase heldPickup))
        {
            heldPickup.Drop();
        }
    }

    public void Use(Transform hand)
    {
        // Get reference to held pickup
        if (GetHeldPickup(hand, out PickupBase heldPickup))
        {
            heldPickup.Use();
        }
    }

    public void Throw(Transform hand)
    {
        // Get reference to held pickup
        if (GetHeldPickup(hand, out PickupBase heldPickup))
        {
            heldPickup.Throw();
        }
    }

    // Return Methods

    public bool GetNearestPickup(out PickupBase nearestPickup)
    {
        // Get a list of nearby colliders
        nearestPickup = Physics.OverlapSphere(transform.position, 2f)

            // Narrow down list to unparented pickups
            .Where(collider => collider.transform.parent == null && collider.CompareTag("Pickup"))

            // Sort list by distance
            .OrderBy(collider => Vector3.Distance(transform.position, collider.transform.position))

            // Get first (nearest) pickup from sorted list
            .FirstOrDefault()?.transform.GetComponent<PickupBase>();

        return nearestPickup != null;
    }

    public bool GetHeldPickup(Transform hand, out PickupBase heldPickup)
    {
        // Returns whatever pickup is in the hand or null if the hand is empty
        heldPickup = hand.childCount > 0 ? hand.GetChild(0).GetComponent<PickupBase>() : null;

        return heldPickup != null;
    }

    public bool IsFull(Transform hand)
    {
        // Returns true if hand has a child
        return hand.childCount > 0;
    }

    public bool IsEmpty(Transform hand)
    {
        // Returns true if hand has no children
        return hand.childCount == 0;
    }
}
