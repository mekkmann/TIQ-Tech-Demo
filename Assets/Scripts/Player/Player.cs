using UnityEngine;

public class Player : Character
{
    public PlayerControls PlayerControls { get; private set; }

    private void Awake()
    {
        PlayerControls = new();
        currentHealth = maxHealth;
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerCombat>().enabled = true;
    }

    protected override void Die()
    {
        if (isDead) return;

        base.Die();

        GetComponent<Animator>().SetBool("isDead", isDead);
    }
}
