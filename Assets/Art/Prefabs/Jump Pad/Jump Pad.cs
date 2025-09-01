using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // Fields

    public float jumpForce = 25f;

    // Collider Methods

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Rigidbody otherRb))
        {
            otherRb.linearVelocity = new Vector3(otherRb.linearVelocity.x, jumpForce, otherRb.linearVelocity.z);
        }
    }
}
