using UnityEngine;

public class IceStaff : PickupBase
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

            // Check for state controller
            if (other.TryGetComponent(out StateController stateController))
            {
                stateController.SetState(ObjectState.Frozen);
            }
        }
    }
}
