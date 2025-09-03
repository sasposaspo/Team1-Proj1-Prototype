using System.Collections;
using UnityEditor;
using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    // Fields

    protected Rigidbody rb;
    protected Collider col;
    protected StateController stateController;

    protected Transform pickupModel;

    protected float hoverMinY;
    protected float hoverMaxY;

    public bool canInteract = true;
    public bool beingThrown = false;

    private GameObject previousParent;

    // Private Methods

    protected void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Get pickup model
        pickupModel = transform.GetChild(0);

        // Get lower & upper positions for the hover animation
        hoverMinY = pickupModel.localPosition.y - 0.25f;
        hoverMaxY = pickupModel.localPosition.y + 0.25f;

        // Start idle animations
        StartCoroutine(RotateAnimation());
        StartCoroutine(HoverAnimationLoop());
    }

    protected void Update()
    {
        // Keep the pickup standing upright when idle
        if (IsIdle() == true) { transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f); }


    }

    protected void TogglePhysics(bool enable)
    {
        rb.isKinematic = !enable; // When isKinematic is disabled, the rigidbody will respond to physics

        // Set rigidbody to interpolate when responding to physics
        rb.interpolation = enable ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
    }

    protected void ToggleCollision(bool enable)
    {
        // Enable or disable collision
        col.enabled = enable;
    }

    protected void ToggleShadows(bool enable)
    {
        // Get mesh renderer from pickup model
        MeshRenderer renderer = pickupModel.GetComponent<MeshRenderer>();

        // Enable or disable shadows
        renderer.shadowCastingMode = enable ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    // Public Methods

    public void PickUp(Transform hand)
    {
        if (canInteract == true)
        {
            TogglePhysics(false);
            ToggleShadows(false);
            ToggleCollision(false);

            // Set parent to hand
            transform.SetParent(hand);

            // Start pickup animation
            StartCoroutine(PickupAnimation());
        }
    }

    public void Drop()
    {
        if (canInteract == true)
        {
            TogglePhysics(true);
            ToggleShadows(true);
            ToggleCollision(true);

            // Unparent pickup
            transform.SetParent(null);
        }
    }

    public void Throw()
    {
        if (canInteract == true)
        {
            // Store reference to whoever threw pickup
            previousParent = transform.root.gameObject;

            Drop();

            // Set flags
            beingThrown = true;
            canInteract = false;

            // Get desired throw direction & force
            Vector3 desiredThrow = (transform.forward * 50f) + (transform.up * 5f);

            // Apply desired throw to rigid body
            rb.AddForce(desiredThrow, ForceMode.Impulse);
        }
    }

    // Abstract Methods

    public abstract void Use();

    // Collision Methods

    private void OnCollisionEnter(Collision collision)
    {
        if (beingThrown == true)
        {
            // Check if collided game object threw this pickup
            if (collision.transform.root.gameObject == previousParent) { return; }

            // Check if collided game object has a health component
            if (collision.gameObject.TryGetComponent(out HealthBase otherHealth))
            {
                // Deal damage
                otherHealth.ModifyHealth(-1);
            }

            // Set flags
            beingThrown = false;
            canInteract = true;
        }
    }

    // Coroutines

    protected IEnumerator RotateAnimation()
    {
        // Get rotation in degrees per second
        Vector3 rotation = new Vector3(0f, 90f, 0f);

        while (true) // Runs forever unless stopped
        {
            if (IsIdle() == true)
            {
                // Rotate pickup model over time
                pickupModel.Rotate(rotation * Time.deltaTime);
            }

            yield return null; // Wait for next frame
        }
    }

    protected IEnumerator HoverAnimationLoop()
    {
        bool isMovingDownward = true;

        while (true) // Runs forever unless stopped
        {
            // Toggle state
            isMovingDownward = !isMovingDownward;

            // Get target position based on state
            float targetY = isMovingDownward ? hoverMinY : hoverMaxY;

            // Start animation and wait for it to finish
            yield return StartCoroutine(HoverAnimation(targetY));
        }
    }

    protected IEnumerator HoverAnimation(float targetY)
    {
        // Customize animation curve
        AnimationCurve smoothAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        smoothAnimationCurve.postWrapMode = WrapMode.PingPong;

        float duration = 1f;
        float startY = pickupModel.localPosition.y;

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration;)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            float currentY = Mathf.Lerp(startY, targetY, smoothAnimationCurve.Evaluate(time));

            if (IsIdle() == true)
            {
                // Apply animation
                pickupModel.localPosition = new Vector3(pickupModel.localPosition.x, currentY, pickupModel.localPosition.z);

                // Progress animation
                elapsedTime += Time.deltaTime;
            }

            yield return null; // Wait for next frame
        }

        if (IsIdle() == true)
        {
            // Ensure finished animation state
            pickupModel.localPosition = new Vector3(pickupModel.localPosition.x, targetY, pickupModel.localPosition.z);
        }
    }

    protected IEnumerator PickupAnimation()
    {
        float duration = 0.15f;

        // Store position & rotation
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        // Store position & rotation
        Vector3 pickupModelStartPos = pickupModel.localPosition;
        Quaternion pickupModelStartRot = pickupModel.localRotation;

        // Get target positions and rotations
        Vector3 targetPos = Vector3.zero;
        Quaternion targetRot = Quaternion.Euler(Vector3.zero);

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, time);
            Quaternion currentRot = Quaternion.Lerp(startRot, targetRot, time);

            // Interpolate over time
            Vector3 pickupModelCurrentPos = Vector3.Lerp(pickupModelStartPos, targetPos, time);
            Quaternion pickupModelCurrentRot = Quaternion.Lerp(pickupModelStartRot, targetRot, time);

            // Apply animations
            transform.localPosition = currentPos;
            transform.localRotation = currentRot;

            // Apply animations
            pickupModel.localPosition = currentPos;
            pickupModel.localRotation = currentRot;

            yield return null; // Wait for next frame
        }

        // Ensure finished animation state
        transform.localPosition = targetPos;
        transform.localRotation = targetRot;

        // Ensure finished animation state
        pickupModel.localPosition = targetPos;
        pickupModel.localRotation = targetRot;
    }

    // Return Methods

    protected bool IsIdle()
    {
        return canInteract == true && transform.parent == null;
    }
}
