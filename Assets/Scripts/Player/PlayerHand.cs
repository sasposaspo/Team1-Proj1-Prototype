using UnityEngine;

public class PlayerHand : HandBase
{
    // Fields

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Use();
        }
    }

    private void LateUpdate()
    {
        IgnoreParentScale();
    }

    private void IgnoreParentScale()
    {
        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(transform.localScale.x, 1f / parentScale.y, transform.localScale.z);
    }
}
