using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    public float baseSpeed = 10f;

    private Vector3 _movement;
    private Quaternion _rotation = Quaternion.identity;
    private Animator _animator;
    private Rigidbody _rigidbody;

    private bool _isMoving;

    #region Input
    private InputAction _move;
    private InputAction _roll;
    #endregion

    private void OnEnable()
    {
        PlayerControls temp = GetComponent<Player>().PlayerControls;
        _move = temp.Player.Move;
        _move.Enable();

        _roll = temp.Player.Roll;
        _roll.Enable();
        _roll.performed += Roll;
    }
    private void OnDisable()
    {
        _move.Disable();
        _roll.Disable();
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {

        // TODO: Try to see if movement can be transfered to inputaction callback
        Vector2 tempMove = _move.ReadValue<Vector2>();

        _movement.Set(tempMove.x, 0f, tempMove.y);
        _movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(_movement.x, 0f);
        bool hasVerticalInput = !Mathf.Approximately(_movement.y, 0f);
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

    #region Rolling
    private void Roll(InputAction.CallbackContext context)
    {
        if (!_isMoving) return;

        _animator.SetTrigger("roll");
    }
    public void SetRollSpeed()
    {
        baseSpeed *= 2f;
    }
    public void RemoveRollSpeed()
    {
        baseSpeed /= 2f;
    }
    #endregion
}
