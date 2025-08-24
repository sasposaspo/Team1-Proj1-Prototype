using UnityEngine;

public class Rotation : MonoBehaviour
{
    // Fields

    public float speed = 1.0f;
    public Vector3 rotation = Vector3.zero;

    // Methods

    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime * speed);
    }
}
