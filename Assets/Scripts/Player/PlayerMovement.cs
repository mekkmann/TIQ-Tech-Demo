using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    public float baseSpeed = 10f;

    private Vector3 _movement;
    private Quaternion _rotation = Quaternion.identity;
    private Animator _animator;
    private Rigidbody _rigidbody;

    private bool _isMoving;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // TODO: Refactor roll input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger("roll");
        }
    }

    void FixedUpdate()
    {
        // TODO: Refactor movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _movement.Set(horizontal, 0f, vertical);
        _movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        _isMoving = hasHorizontalInput || hasVerticalInput;
        _animator.SetBool("isMoving", _isMoving);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, _movement, turnSpeed * Time.deltaTime, 0f);
        _rotation = Quaternion.LookRotation(desiredForward);
    }

    private void OnAnimatorMove()
    {
        // Protects against accidental rotation or movement when idling
        if (!_isMoving) return;

        _rigidbody.MovePosition(_rigidbody.position + baseSpeed * Time.deltaTime * _movement);
        _rigidbody.MoveRotation(_rotation);
    }

    public void SetRollSpeed()
    {
        baseSpeed *= 2f;
    }
    public void RemoveRollSpeed()
    {
        baseSpeed /= 2f;
    }
}
