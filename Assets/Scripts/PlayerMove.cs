using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class PlayerMove : MonoBehaviour
{
    // Fields

    public float speed = 20f;
    public float jumpForce = 15f;

    private Collider col;
    private Rigidbody rb;

    private new Transform camera;

    private Vector3 standingHeight;
    private Vector3 crouchHeight;

    public int maxAirJumps = 1;
    private int airJumpsLeft;

    // Methods

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        camera = GetComponentInChildren<Camera>().transform;

        standingHeight = transform.localScale;
        crouchHeight = new Vector3(standingHeight.x, standingHeight.y / 2, standingHeight.z);
    }

    private void FixedUpdate()
    {
        Move(); // Not ran every frame to avoid issues w/ physics
    }

    private void Update()
    {
        Jump();
        Crouch();
    }

    private void Move()
    {
        // Get horizontal & vertical movement input
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // Combine horizontal & vertical movement input
        Vector3 inputDirection = new Vector3(xInput, 0f, zInput);

        // Rotate input direction by camera's Y rotation
        Vector3 movementDirection = Quaternion.Euler(0f, camera.eulerAngles.y, 0f) * inputDirection;

        // Times movement direction by speed & prevent player from moving faster diagonally
        Vector3 movement = Vector3.ClampMagnitude(movementDirection, 1f) * speed;

        // Keep existing vertical velocity
        movement.y = rb.linearVelocity.y;

        rb.linearVelocity = movement; // Apply movement to Rigidbody
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && airJumpsLeft > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z); // Add upwards force
            airJumpsLeft--;
        }

        if (IsGrounded() == true)
        {
            airJumpsLeft = maxAirJumps;
        }
    }

    private void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            StopAllCoroutines();
            StartCoroutine(ScaleAnimation(crouchHeight));
        }
        if (Input.GetButtonUp("Crouch"))
        {
            StopAllCoroutines();
            StartCoroutine(ScaleAnimation(standingHeight, ()=> CheckCollisionAt(PlayerColliderTop()) == false));
        }
    }

    // Coroutines

    private IEnumerator ScaleAnimation(Vector3 targetScale, Func<bool> canProgress = null) // Used for crouching & uncrouching
    {
        float duration = 0.1f;
        Vector3 startHeight = transform.localScale;

        // Track the elapsed time of the animation
        for (float elapsedTime = 0f; elapsedTime < duration;)
        {
            if (canProgress == null || canProgress() == true) // If condition is null or met
            {
                elapsedTime += Time.deltaTime; // Progress animation
            }

            float time = elapsedTime / duration; // Normalize elapsed time

            // Interpolate over time
            Vector3 currentHeight = Vector3.Lerp(startHeight, targetScale, time);

            transform.localScale = currentHeight; // Apply animation

            yield return null; // Wait for next frame
        }

        transform.localScale = targetScale; // Ensure finished animation state
    }

    // Return Methods

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

    private bool IsGrounded()
    {
        // Returns true if any collider that is not the player's collider is detected at the bottom point of the player's collider
        return CheckCollisionAt(PlayerColliderBottom());
    }
}
