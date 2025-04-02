using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    private Animator _animator;
    public PlayerControls PlayerControls { get; private set; }

    #region Input
    InputAction _look;
    #endregion

    private void Awake()
    {
        PlayerControls = new();

        currentHealth = maxHealth;
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerCombat>().enabled = true;
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _look = PlayerControls.Player.Roll;
        _look.Enable();
        //_look.performed += Look;
    }
    private void OnDisable()
    {
        _look.Disable();
    }

    // TODO: Figure out how to make a 3rd person camera controller
    //private void Look(InputAction.CallbackContext context)
    //{

    //}

    protected override void Die()
    {
        if (isDead) return;

        base.Die();

        _animator.SetBool("isDead", isDead);
    }
}
