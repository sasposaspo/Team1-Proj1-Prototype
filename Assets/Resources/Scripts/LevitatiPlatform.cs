using Unity.VisualScripting;
using UnityEngine;

public class LevitatePlatform : MonoBehaviour
{
    // Fields

    [Header("Vertical Motion")]
    [Tooltip("How high the platform floats up and down.")]
    public float verticalAmplitude = 0.5f;
    [Tooltip("Speed of vertical floating.")]
    public float verticalFrequency = 1f;

    [Header("Horizontal Motion")]
    [Tooltip("How far the platform drifts left and right.")]
    public float horizontalAmplitude = 0.25f;
    [Tooltip("Speed of horizontal drifting.")]
    public float horizontalFrequency = 0.5f;

    [Header("Tilt Motion")]
    [Tooltip("Maximum tilt in degrees.")]
    public float tiltAngle = 5f;
    [Tooltip("Speed of tilting motion.")]
    public float tiltFrequency = 1f;

    private Vector3 startPos;
    private Quaternion startRot;
    private float phaseOffset;

    private float motionTime = 0f;

    // Methods

    private void Awake()
    {
        // Random offset so each platform floats and tilts independently
        phaseOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    private void Update()
    {
        if (GetComponent<StateController>().currentState != ObjectState.Frozen)
        {
            motionTime += Time.deltaTime;

            Vector3 tempPos = startPos;

            // Vertical floating
            tempPos.y += Mathf.Sin(motionTime * verticalFrequency + phaseOffset) * verticalAmplitude;

            // Horizontal drifting
            tempPos.x += Mathf.Sin(motionTime * horizontalFrequency + phaseOffset) * horizontalAmplitude;

            transform.position = tempPos;

            // Tilt rotation
            float tiltX = Mathf.Sin(motionTime * tiltFrequency + phaseOffset) * tiltAngle;
            float tiltZ = Mathf.Cos(motionTime * tiltFrequency + phaseOffset) * tiltAngle;

            transform.rotation = startRot * Quaternion.Euler(tiltX, 0f, tiltZ);
        }
    }
}