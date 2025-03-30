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

    // CONSTRUCTOR
    //public PlayerMovement(InputAction move, InputAction roll)
    //{
    //    _move = move;
    //    _roll = roll;
    //}

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
        // Joystick
        //Vector2 tempMove = _move.ReadValue<Vector2>();

        //_movement.Set(tempMove.x, 0f, tempMove.y);
        //_movement.Normalize();

        //bool hasHorizontalInput = !Mathf.Approximately(_movement.x, 0f);
        //bool hasVerticalInput = !Mathf.Approximately(_movement.y, 0f);
        //_isMoving = hasHorizontalInput || hasVerticalInput;
        //_animator.SetBool("isMoving", _isMoving);

        //Vector3 desiredForward = Vector3.RotateTowards(transform.forward, _movement, turnSpeed * Time.deltaTime, 0f);
        //_rotation = Quaternion.LookRotation(desiredForward);

        // quick testing with keyboard mouse
        float translation = Input.GetAxis("Vertical") * baseSpeed;
        float rotation = Input.GetAxis("Horizontal") * turnSpeed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);
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
