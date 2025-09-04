using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out HealthBase health))
        {
            health.ModifyHealth(-1);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out HealthBase health))
        {
            health.ModifyHealth(-1);
        }
    }
}
