using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyController : HealthBase
{
    // Fields

    private NavMeshAgent ai;
    private EnemyHands enemyHands;

    // Methods

    private void Start()
    {
        ai = GetComponent<NavMeshAgent>();
        enemyHands = GetComponentInChildren<EnemyHands>();
    }

    private void Update()
    {
        GetDestination();
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
                Vector3 directionAway = (ai.transform.position - player2.position).normalized;

                ai.SetDestination(ai.transform.position + (directionAway * 20f));
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

    protected override void Die()
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
