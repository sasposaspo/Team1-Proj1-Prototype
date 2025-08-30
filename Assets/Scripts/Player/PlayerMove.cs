using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviour
{
    // Fields

    public float speed = 8f;
    public float jumpForce = 12f;

    private Collider col;
    private Rigidbody rb;

    private new Transform camera;

    private int airJumpsLeft;
    private int maxAirJumps = 1;
    private Coroutine coyoteJumpWindow = null;

    private float standingHeight;
    private float crouchingHeight;
    private Coroutine heightAnimation = null;

    private bool onIce;
    private bool isGrounded;


    // Methods

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        camera = GetComponentInChildren<Camera>().transform;

        standingHeight = transform.localScale.y;
        crouchingHeight = standingHeight / 2;
    }

    private void Update()
    {
        SetIsGrounded();

        if (GameManager.gameOver == false)
        {
            Jump();
            Crouch();
        }
    }

    private void FixedUpdate() // Not ran every frame to avoid issues w/ physics
    {
        if (GameManager.gameOver == true) { return; }

        if (onIce == true) { Slide(); }
        else { Move(); }
    }

    private void Move()
    {
        // Get desired horizontal velocity
        Vector3 targetVelocity = MovementDirection() * speed;

        if (isGrounded == true)
        {
            Rigidbody platformRb = PlatformRigidBody();

            if (platformRb != null)
            {
                targetVelocity += platformRb.linearVelocity;
            }
        }

        // Get velocity change need
        Vector3 changeDifference = targetVelocity - rb.linearVelocity;

        // Keep existing vertical velocity
        changeDifference.y = 0f;

        // Apply instant velocity change
        rb.AddForce(changeDifference, ForceMode.VelocityChange);
    }

    private void Slide()
    {
        // Get desired horizontal velocity
        Vector3 targetAcceleration = MovementDirection() * speed;

        // Keep existing vertical velocity
        targetAcceleration.y = 0f;

        // Accelerate over time
        rb.AddForce(targetAcceleration, ForceMode.Acceleration);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || coyoteJumpWindow != null) // Ground or coyote jump
            {
                // Add upwards force
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }
            else if (isGrounded == false && airJumpsLeft > 0) // Air jump
            {
                // Add upwards force
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                airJumpsLeft--;
            }
        }

        // Air jump refill
        if (isGrounded == true) { airJumpsLeft = maxAirJumps; }
    }

    private void SetIsGrounded()
    {
        // Store previous grounded state
        bool wasGrounded = isGrounded;

        // Update grounded state
        isGrounded = CheckCollisionAt(PlayerColliderBottom());

        if (isGrounded == true) // If the player is on the ground
        {
            // Stop any coyote jump windows
            if (coyoteJumpWindow != null)
            {
                StopCoroutine(coyoteJumpWindow);
                coyoteJumpWindow = null;
            }
        }
        else if (wasGrounded == true) // If player has just left the ground
        {
            // Start coyote jump window
            coyoteJumpWindow = StartCoroutine(CoyoteJumpWindow());
        }
    }

    private void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            // Stop any height animation
            if (heightAnimation != null) { StopCoroutine(heightAnimation); }

            // Start new height animation
            heightAnimation = StartCoroutine(HeightAnimation(crouchingHeight));
        }
        if (Input.GetButtonUp("Crouch"))
        {
            // Stop any height animation
            if (heightAnimation != null) { StopCoroutine(heightAnimation); }

            // Start new height animation
            heightAnimation = StartCoroutine(HeightAnimation(standingHeight, ()=> CheckCollisionAt(PlayerColliderTop()) == false));
        }
    }

    // Coroutines

    private IEnumerator HeightAnimation(float targetHeight, Func<bool> canProgress = null) // Used for crouching & uncrouching
    {
        float duration = 0.1f;
        float startHeight = transform.localScale.y;

        // Track the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration;)
        {
            if (canProgress == null || canProgress() == true) // If condition is null or met
            {
                elapsedTime += Time.deltaTime; // Progress animation
            }

            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            float currentHeight = Mathf.Lerp(startHeight, targetHeight, time);

            transform.localScale = new Vector3(transform.localScale.x, currentHeight, transform.localScale.z); // Apply animation

            yield return null; // Wait for next frame
        }

        // Ensure finished animation state
        transform.localScale = new Vector3(transform.localScale.x, targetHeight, transform.localScale.z);
    }

    private IEnumerator StopSlide()
    {
        // Wait until player touches the ground
        while (isGrounded == false) { yield return null; }

        onIce = false;
    }

    private IEnumerator CoyoteJumpWindow()
    {
        // Give player a small window to jump
        yield return new WaitForSeconds(0.2f);
        coyoteJumpWindow = null;
    }

    // Return Methods

    private Vector3 MovementDirection()
    {
        // Get horizontal & vertical movement input
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // Combine horizontal & vertical movement input
        Vector3 inputDirection = new Vector3(xInput, 0f, zInput);

        // Prevent player from moving faster diagonally
        Vector3 normalizedInputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        // Rotate input direction by camera's Y rotation
        Vector3 movementDirection = Quaternion.Euler(0f, camera.eulerAngles.y, 0f) * normalizedInputDirection;

        return movementDirection;
    }

    private bool CheckCollisionAt(Vector3 position)
    {
        // Returns true if any collider that is not the player's collider is detected
        return Physics.OverlapSphere(position, 0.1f).Any(collider => collider != col);
    }

    private Vector3 PlayerColliderBottom()
    {
        // Returns the bottom point of the player's collider
        return new Vector3(transform.position.x, col.bounds.min.y, transform.position.z);
    }

    private Vector3 PlayerColliderTop()
    {
        // Returns the top point of the player's collider
        return new Vector3(transform.position.x, col.bounds.max.y, transform.position.z);
    }

    private Rigidbody PlatformRigidBody()
    {
        // Returns whatever rigidbody is under the player
        return Physics.OverlapSphere(PlayerColliderBottom(), 0.1f)
            .Where(collider => collider != col && collider.attachedRigidbody != null)
            .Select(collider => collider.attachedRigidbody)
            .FirstOrDefault();
    }

    // Collision Methods

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Ice ice))
        {
            onIce = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Ice ice))
        {
            StartCoroutine(StopSlide());
        }
    }
}
