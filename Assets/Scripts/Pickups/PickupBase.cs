using System.Collections;
using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    // Fields

    protected Rigidbody rb;
    protected Collider col;

    protected Transform pickupModel;

    protected float hoverMinY;
    protected float hoverMaxY;

    protected bool canInteract = true;

    protected Transform previousParent;
    protected bool beingThrown = false;

    // Private Methods

    protected void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Get pickup model
        pickupModel = transform.GetChild(0);

        // Get the lower & upper positions for the hover animation
        hoverMinY = pickupModel.localPosition.y - 0.25f;
        hoverMaxY = pickupModel.localPosition.y + 0.25f;

        Idle();
    }

    protected void Update()
    {
        // Keep the pickup standing upright when idle
        if (transform.parent == null && canInteract == true) { transform.rotation = Quaternion.Euler(Vector3.zero); }
    }

    protected void TogglePhysics(bool enable)
    {
        rb.isKinematic = !enable; // When isKinematic is disabled, the rigidbody will respond to physics

        // Set rigidbody to interpolate when responding to physics
        rb.interpolation = enable ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;

        col.enabled = enable; // Enable the collider when responding to physics
    }

    protected void Idle()
    {
        StartCoroutine(RotateAnimation());
        StartCoroutine(HoverAnimationLoop());
    }

    // Public Methods

    public void PickUp(Transform hand)
    {
        if (canInteract == true)
        {
            TogglePhysics(false);
            transform.SetParent(hand);

            // Center the pickup model
            pickupModel.localPosition = Vector3.zero;

            StopAllCoroutines();
            StartCoroutine(PickupAnimation());

            pickupModel.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    public void Drop()
    {
        if (canInteract == true)
        {
            TogglePhysics(true);
            transform.SetParent(null);

            Idle();

            pickupModel.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

    public void Throw()
    {
        if (canInteract == true)
        {
            previousParent = transform.parent.root;
            Debug.Log(previousParent.name);

            TogglePhysics(true);
            transform.SetParent(null);

            beingThrown = true;
            canInteract = false;
            rb.AddForce(transform.forward * 50f, ForceMode.Impulse);
            rb.AddForce(transform.up * 5f, ForceMode.Impulse);
        }
    }

    public virtual void Use()
    {

    }

    // Collision Methods

    private void OnCollisionEnter(Collision collision)
    {
        if (beingThrown == true)
        {
            Debug.Log(collision.gameObject.name);

            // Thrown pickup will not deal damage to whoever threw it
            if (collision.gameObject.name == previousParent.name) { return; }

            //
            if (collision.gameObject.TryGetComponent(out HealthBase otherHealth))
            {
                otherHealth.ModifyHealth(-1);
            }

            beingThrown = false;
            canInteract = true;

            Idle();
        }
    }

    // Coroutines

    protected IEnumerator PickupAnimation()
    {
        float duration = 0.15f;

        // Store start position & start rotation
        transform.GetLocalPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);

        Vector3 targetPosition = Vector3.zero;
        Quaternion targetRotation = Quaternion.Euler(Vector3.zero);

        // Track & increase the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate position & rotation over time
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, time);
            Quaternion currentRotation = Quaternion.Lerp(startRotation, targetRotation, time);

            // Apply interpolations to transform
            transform.SetLocalPositionAndRotation(currentPosition, currentRotation);

            yield return null; // Wait for next frame
        }

        // Ensure target position & target rotation are set
        transform.SetLocalPositionAndRotation(targetPosition, targetRotation);
    }

    protected IEnumerator RotateAnimation()
    {
        Vector3 rotation = new Vector3(0f, 90f, 0f);

        while (true) // Runs forever unless stopped
        {
            // Rotate pickup model
            pickupModel.Rotate(rotation * Time.deltaTime);

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
        for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
        {
            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            float currentY = Mathf.Lerp(startY, targetY, smoothAnimationCurve.Evaluate(time));

            // Apply animation
            pickupModel.localPosition = new Vector3(pickupModel.localPosition.x, currentY, pickupModel.localPosition.z);

            yield return null; // Wait for next frame
        }

        // Ensure finished animation state
        pickupModel.localPosition = new Vector3(pickupModel.localPosition.x, targetY, pickupModel.localPosition.z);
    }
}
