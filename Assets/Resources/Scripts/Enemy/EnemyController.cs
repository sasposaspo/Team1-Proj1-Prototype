using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyController : HealthBase
{
    // Fields

    public float acceleration = 10f;
    public float maxSpeed = 5f;

    private Rigidbody rb;
    private NavMeshAgent ai;
    private EnemyHands enemyHands;

    // Methods

    private void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody>();
        ai = GetComponent<NavMeshAgent>();
        enemyHands = GetComponentInChildren<EnemyHands>();

        ai.updatePosition = false;
        ai.updateRotation = false;
    }

    private void Update()
    {
        GetDestination();

        if (rb.isKinematic == false)
        {
            enemyHands.AimHands();
            enemyHands.HandleThrow();
            enemyHands.HandlePickUp();
        }
    }

    private void FixedUpdate() // Not ran every frame to avoid issues w/ physics
    {
        Vector3 desiredVelocity = ai.desiredVelocity;
        Vector3 force = desiredVelocity - rb.linearVelocity;

        rb.AddForce(force * acceleration, ForceMode.Acceleration);

        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }
    }

    private void GetDestination()
    {
        if (PlayerIsNearby() == true)
        {
            if (enemyHands.EmptyHanded() == true && GetNearestTarget("Pickup", out Transform pickup))
            {
                ai.SetDestination(pickup.position);
                return;
            }

            if (enemyHands.EmptyHanded() == false && GetNearestTarget("Player", out Transform player))
            {
                ai.SetDestination(player.position);
                return;
            }

            if (enemyHands.EmptyHanded() == true && GetNearestTarget("Player", out Transform player2))
            {
                Vector3 directionAway = (transform.position - player2.position).normalized;

                ai.SetDestination(transform.position + (directionAway * 20f));
            }
        }

        if (PlayerIsNearby() == false)
        {
            if (enemyHands.FullHanded() == false && GetNearestTarget("Pickup", out Transform pickup))
            {
                ai.SetDestination(pickup.position);
                return;
            }
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    // Return Methods

    public bool GetNearestTarget(string targetTag, out Transform nearestTarget)
    {
        // Get a list of nearby colliders
        nearestTarget = Physics.OverlapSphere(transform.position, 20f)

            // Narrow down list to unparented targets
            .Where(collider => collider.transform.parent == null && collider.CompareTag(targetTag))

            // Sort list by distance
            .OrderBy(collider => Vector3.Distance(transform.position, collider.transform.position))

            // Get first (nearest) target from sorted list
            .FirstOrDefault()?.transform;

        return nearestTarget != null;
    }

    public bool PlayerIsNearby()
    {
        // Get a list of nearby colliders
        Transform player = Physics.OverlapSphere(transform.position, 20f)

            // Narrow down list to player
            .Where(collider => collider.CompareTag("Player"))

            // Get player from list
            .FirstOrDefault()?.transform;

        if (player != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
