using UnityEngine;

public class Player : Character
{
    private bool _isGrounded = false;
    private bool _isRunning = false;

    [SerializeField] private Animator _animator;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _animator.SetFloat("moveSpeed", 0.15f);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _animator.SetFloat("moveSpeed", 0.05f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _animator.SetBool("isRunning", true);
        }
    }

    protected override void Die()
    {
        if (isDead) return;


        base.Die();

        _animator.SetBool("isDead", isDead);
    }
}
