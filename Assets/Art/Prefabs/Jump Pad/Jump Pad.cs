using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // Fields

    public float jumpForce = 25f;

    // Collider Methods

    private void OnCollisionEnter(Collision collision)
    {
        // If other game object has a rigid body
        if (collision.gameObject.TryGetComponent(out Rigidbody otherRb))
        {
            // Add upwards force
            otherRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
}
