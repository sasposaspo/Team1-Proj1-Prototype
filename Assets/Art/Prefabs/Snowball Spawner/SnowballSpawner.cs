using UnityEngine;

public class SnowballSpawner : MonoBehaviour
{
    // Fields

    public GameObject snowball;
    public float spawnRateInSeconds = 7f;

    // Methods

    private void Start()
    {
        SpawnSnowball();
    }

    private void SpawnSnowball()
    {
        Instantiate(snowball, transform.position, Quaternion.identity);

        Invoke(nameof(SpawnSnowball), spawnRateInSeconds);
    }
}
