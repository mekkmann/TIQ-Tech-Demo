using UnityEngine;

public class Player : Character
{
    private Animator _animator;
    private Rigidbody _rb;
    public PlayerControls PlayerControls { get; private set; }

    private void Awake()
    {
        PlayerControls = new();
        currentHealth = maxHealth;
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerCombat>().enabled = true;
        _animator = GetComponent<Animator>();
    }

    protected override void Die()
    {
        if (isDead) return;

        base.Die();

        _animator.SetBool("isDead", isDead);
    }
}
