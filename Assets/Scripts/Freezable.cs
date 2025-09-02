using UnityEngine;
using UnityEngine.AI;

public class Freezable : MonoBehaviour
{
    // Fields

    public GameObject iceCubePrefab;
    private GameObject iceCube;

    // Methods

    public void Freeze()
    {
        // Disable any rigid body
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.interpolation = RigidbodyInterpolation.None;
            rb.isKinematic = true;
        }

        // Disable any nav mesh agent
        if (TryGetComponent(out NavMeshAgent ai))
        {
            ai.enabled = false;
        }

        // Disable any enemy controller
        if (TryGetComponent(out EnemyController enemyController))
        {
            enemyController.enabled = false;
        }

        // Disable any collider
        if (TryGetComponent(out Collider collider))
        {
            collider.enabled = false;
        }

        SpawnIceCube();

        // Unfreeze in 5 seconds
        Invoke(nameof(UnFreeze), 5f);
    }

    public void UnFreeze()
    {
        // Enable any rigid body
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.isKinematic = false;
        }

        // Enable any nav mesh agent
        if (TryGetComponent(out NavMeshAgent ai))
        {
            ai.enabled = true;
        }

        // Enable any enemy controller
        if (TryGetComponent(out EnemyController enemyController))
        {
            enemyController.enabled = false;
        }

        // Enable any collider
        if (TryGetComponent(out Collider colider))
        {
            colider.enabled = true;
        }

        // Unparent from ice cube
        transform.SetParent(null, worldPositionStays: true);

        Destroy(iceCube);
    }

    private void SpawnIceCube()
    {
        // Get the bounds of the game object
        Bounds bounds = new Bounds(transform.position, Vector3.zero);

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(r.bounds);
        }

        // Instantiate ice cube
        iceCube = Instantiate(iceCubePrefab, transform.position, Quaternion.identity);

        // Customize ice cube
        iceCube.transform.position = bounds.center;
        iceCube.transform.localScale = bounds.size + (Vector3.one * 1.05f);

        iceCube.transform.rotation = transform.rotation;

        // Set parent to ice cube
        transform.SetParent(iceCube.transform, worldPositionStays: true);
    }
}
