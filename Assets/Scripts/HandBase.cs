using UnityEngine;
using System.Linq;
using System;

public abstract class HandBase : MonoBehaviour
{
    // Methods

    protected virtual void Pickup()
    {
        if (GetNearestPickup(out Transform nearestPickup))
        {
            if (GetHeldPickup(out PickupBase heldPickup))
            {
                heldPickup.Drop();
            }

            PickupBase nearestPickupBase = nearestPickup.GetComponent<PickupBase>(); // Get pickup controller script from pickup

            nearestPickupBase.PickUp(transform);
        }
        else
        {
            if (GetHeldPickup(out PickupBase heldPickup))
            {
                heldPickup.Drop();
            }
        }
    }

    protected virtual void Use()
    {
        if (GetHeldPickup(out PickupBase heldPickup))
        {
            heldPickup.Use();
        }
    }

    // Return Methods

    private bool GetNearestPickup(out Transform nearestPickup)
    {
        nearestPickup = Physics.OverlapSphere(transform.position, 2f) // Search for nearby colliders
            .Where(collider => collider.transform.parent == null && collider.CompareTag("Pickup")) // Check if they are pickups
            .OrderBy(collider => Vector3.Distance(transform.position, collider.transform.position)) // Sort pickups by distance
            .FirstOrDefault()?.transform; // Get first (nearest) pickup from sorted list
            // Question mark can set to null without throwing an error

        return nearestPickup != null;
    }

    private bool GetHeldPickup(out PickupBase heldPickup)
    {
        heldPickup = transform.childCount > 0 ? transform.GetChild(0).GetComponent<PickupBase>() : null;
        return heldPickup != null;
    }
}
