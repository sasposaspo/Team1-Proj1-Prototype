using System.Collections.Generic;
using UnityEngine;

public class FireStaff : PickupBase
{
    // Methods

    public override void Use()
    {
        // Create a ray from where the staff is facing
        Ray ray = new Ray(transform.position, transform.forward);

        // Shoot ray and store hit info
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Store reference to hit transform
            Transform other = hitInfo.transform;

            StateController[] stateControllers = other.GetComponentsInChildren<StateController>();

            if (stateControllers.Length > 0)
            {
                stateControllers[0].SetState(ObjectState.Burning);
            }
        }
    }
}
