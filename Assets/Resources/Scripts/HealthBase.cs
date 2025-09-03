using System.Collections;
using UnityEngine;

public abstract class  HealthBase : MonoBehaviour
{
    // Fields

    public int health = 1;
    public float invincibilityDuration = 1f;

    private bool isInvincible = false;
    private Coroutine invincibilityFrames = null;

    // Virtual Methods

    public virtual void ModifyHealth(int amount)
    {
        if (GameManager.gameOver == true) { return; }

        if (amount < 0 && isInvincible) { return; }

        health += amount;

        if (health <= 0) { Die(); return; }

        if (amount < 0) { TakeDamage(); return; }

        if (amount > 0) { Heal(); }
    }

    protected virtual void Heal()
    {

    }

    protected virtual void TakeDamage()
    {
        if (invincibilityFrames != null)
        {
            StopCoroutine(invincibilityFrames);
        }

        invincibilityFrames = StartCoroutine(InvincibilityFrames());
    }

    // Abstract Methods

    public abstract void Die();

    // Coroutines

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return invincibilityDuration;
        isInvincible = false;
    }
}
