using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected float baseMovementSpeed;
    [SerializeField] protected float movementSpeedMultiplier;
    protected bool isDead = false;

    public bool IsDead { get { return isDead; } }

    protected virtual void TakeDamage(int damageTaken)
    {
        if (isDead) return;

        if (currentHealth >= 0)
        {
            currentHealth -= damageTaken;
            if (currentHealth <= 0)
            {
                Die();
                return;
            }
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
    }

}
