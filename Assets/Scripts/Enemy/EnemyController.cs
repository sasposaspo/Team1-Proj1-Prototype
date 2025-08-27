using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyController : HealthBase
{
    // Fields

    private NavMeshAgent ai;

    private Transform player;

    // Methods

    private void Start()
    {
        ai = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        GetDestination();
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    private void GetDestination()
    {
        if (Vector3.Distance(transform.position, player.position) < 20f)
        {
            ai.SetDestination(player.position);
        }
    }
}
