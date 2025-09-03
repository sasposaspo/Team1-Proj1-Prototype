using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;
using System.Linq;

public class StateController : MonoBehaviour
{
    // Fields

    public bool moveableWhenFrozen = true;

    public ObjectState currentState = ObjectState.Normal;

    private float burnDuration = 5f;
    private float freezeDuration = 3f;

    private bool burnCancelled;
    private bool freezeCancelled;

    private Action onBurnCancelled;
    private Action onFreezeCancelled;

    private GameObject iceCube;

    private List<Material> materials = new List<Material>();
    private List<Color> startColors = new List<Color>();

    // Methods

    private void Start()
    {
        Material iceMat = Resources.Load<Material>("Materials/IceMat");
        PhysicsMaterial icePhysMat = Resources.Load<PhysicsMaterial>("Physics Materials/Ice");

        materials = Materials();

        foreach (Material mat in Materials())
        {
            startColors.Add(mat.color);
        }
    }

    public void SetState(ObjectState desiredState)
    {
        if (desiredState != currentState)
        {
            if (desiredState == ObjectState.Frozen)
            {
                if (currentState == ObjectState.Burning)
                {
                    CancelBurn();
                    currentState = ObjectState.Normal;
                    return;
                }

                if (currentState == ObjectState.Normal)
                {
                    StartFreeze();
                    currentState = ObjectState.Frozen;
                    return;
                }
            }

            if (desiredState == ObjectState.Burning)
            {
                if (currentState == ObjectState.Frozen)
                {
                    CancelFreeze();
                    currentState = ObjectState.Normal;
                    return;
                }

                if (currentState == ObjectState.Normal)
                {
                    StartBurn();
                    currentState = ObjectState.Burning;
                    return;
                }
            }
        }
    }

    private void StartBurn()
    {
        burnCancelled = false;
        onBurnCancelled = () =>
        {
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].color = startColors[i];
            }
        };

        StartCoroutine(Burn());
    }

    private void StartFreeze()
    {
        freezeCancelled = false;
        onFreezeCancelled = () =>
        {
            TogglePhysics(true);
            ToggleCollision(true);
            TogglePickupController(true);
            DestroyIceCube();
        };

        StartCoroutine(Freeze());
    }

    private void CancelBurn()
    {
        burnCancelled = true;
    }

    private void CancelFreeze()
    {
        freezeCancelled = true;
    }

    // Coroutines

    private IEnumerator Burn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < burnDuration)
        {
            float time = elapsedTime / burnDuration;

            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].color = Color.Lerp(startColors[i], Color.red, time);
            }

            if (burnCancelled)
            {
                onBurnCancelled?.Invoke();
                yield break; // Break out of coroutine
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        Destroy(transform.gameObject);
    }

    private IEnumerator Freeze()
    {
        TogglePhysics(false);
        ToggleCollision(false);
        TogglePickupController(false);
        SpawnIceCube();

        float elapsedTime = 0f;

        while (elapsedTime < freezeDuration)
        {
            if (freezeCancelled)
            {
                onFreezeCancelled?.Invoke();
                yield break; // Break out of coroutine
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        TogglePhysics(true);
        ToggleCollision(true);
        TogglePickupController(true);
        DestroyIceCube();

        currentState = ObjectState.Normal;
    }

    // Helper Methods

    private void SpawnIceCube()
    {
        // Get the bounds of the game object
        Bounds bounds = new Bounds(transform.position, Vector3.zero);

        // Expand the bounds of the game object to include children
        foreach (Renderer r in transform.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(r.bounds);
        }

        // Create ice cube
        iceCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Customize position, rotation, & scale
        iceCube.transform.SetPositionAndRotation(bounds.center, transform.rotation);
        iceCube.transform.localScale = bounds.size + (Vector3.one * 1.05f);

        // Get material & physics material for ice cube
        Material iceMat = Resources.Load<Material>("Materials/IceMat");
        PhysicsMaterial icePhysMat = Resources.Load<PhysicsMaterial>("Physics Materials/Ice");

        // Set material & physics material
        iceCube.GetComponent<Renderer>().material = iceMat;
        iceCube.GetComponent<Collider>().material = icePhysMat;

        if (moveableWhenFrozen == true)
        {
            // Add & customize rigid body
            Rigidbody iceCubeRb = iceCube.AddComponent<Rigidbody>();
            iceCube.GetComponent<Rigidbody>().isKinematic = false;
            iceCube.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            
            // If object has a rigid body, inherit mass
            if (transform.TryGetComponent(out Rigidbody rb)) { iceCubeRb.mass = rb.mass;}
        }

        // Set parent to ice cube
        transform.SetParent(iceCube.transform, worldPositionStays: true);
    }

    private void DestroyIceCube()
    {
        // Unparent from ice cube
        transform.SetParent(null, worldPositionStays: true);

        // Destroy ice cube
        UnityEngine.Object.Destroy(iceCube);
    }

    private void TogglePhysics(bool enable)
    {
        // If object has a rigid body
        if (transform.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = !enable; // When isKinematic is disabled, the rigidbody will respond to physics

            // Set rigidbody to interpolate when responding to physics
            rb.interpolation = enable ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }
    }

    private void ToggleCollision(bool enable)
    {
        // If object has collider
        if (transform.TryGetComponent(out Collider col))
        {
            // Enable or disable collision
            col.enabled = enable;
        }
    }

    private void TogglePickupController(bool enable)
    {
        // If object has pickup controller
        if (transform.TryGetComponent(out PickupBase pickupBase))
        {
            pickupBase.canInteract = enable;

            if (enable == false)
            {
                pickupBase.beingThrown = false;
            }
        }
    }

    // Return Methods

    private List<Material> Materials()
    {
        List<Material> materials = new List<Material>();
        List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material != null && materials.Contains(material) == false)
                {
                    materials.Add(material);
                }
            }
        }

        return materials;
    }
}
