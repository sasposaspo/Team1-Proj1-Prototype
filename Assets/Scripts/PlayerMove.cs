using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class PlayerMove : MonoBehaviour
{
    // Fields

    public float normalSpeed = 8f;
    private float speed = 8f;
    public float jumpForce = 12f;

    private Collider col;
    private Rigidbody rb;

    private new Transform camera;

    public int maxAirJumps = 1;
    private int airJumpsLeft;

    private float standingHeight;
    private float crouchingHeight;

    private Coroutine heightAnimation = null;

    // Methods

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        camera = GetComponentInChildren<Camera>().transform;

        standingHeight = transform.localScale.y;
        crouchingHeight = standingHeight / 2;

        speed = normalSpeed;
    }

    private void FixedUpdate()
    {
        if (GameManager.gameOver == false)
        {
            Move(); // Not ran every frame to avoid issues w/ physics
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (GameManager.gameOver == false)
        {
            Jump();
            Crouch();
        }
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
        if (Input.GetButton("Jump") && IsGrounded() == true)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z); // Add upwards force
        }

        if (Input.GetButtonDown("Jump") && IsGrounded() == false && airJumpsLeft > 0)
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
            StopHeightAnimation();
            heightAnimation = StartCoroutine(HeightAnimation(crouchingHeight));
        }
        if (Input.GetButtonUp("Crouch"))
        {
            StopHeightAnimation();
            heightAnimation = StartCoroutine(HeightAnimation(standingHeight, ()=> CheckCollisionAt(PlayerColliderTop()) == false));
        }
    }

    private void StopHeightAnimation()
    {
        if (heightAnimation != null)
        {
            StopCoroutine(heightAnimation);
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

    // Collision Methods

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            speed = normalSpeed * 3;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            speed = normalSpeed;
        }
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
