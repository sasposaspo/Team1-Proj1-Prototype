using System.Collections;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    private GameManager gameManager;

    // Methods

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public override void Die()
    {
        gameManager.PlayerDeath();
    }
}
