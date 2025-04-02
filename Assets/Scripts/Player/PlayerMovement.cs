using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Jog and Run")]
    [SerializeField] private float _turnSpeed = 20f;
    [SerializeField] private float _baseSpeed = 10f;
    [SerializeField] private float _runSpeed = 15f;

    private Rigidbody _rb;

    private Vector3 _movement;
    private Quaternion _rotation = Quaternion.identity;
    private Animator _animator;

    private bool _isMoving;

    [Header("Rolling")]
    [SerializeField] private float _rollForce = 1f;
    [SerializeField] private bool _canRoll = true;
    [SerializeField] private bool _isRolling = false;
    public bool IsRolling => _isRolling;
    [SerializeField] private float _rollCooldown = 0f;
    [SerializeField] private AnimationClip _rollAnimation;


    #region Input
    private InputAction _move;
    private InputAction _roll;
    private InputAction _run;
    #endregion

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        PlayerControls temp = GetComponent<Player>().PlayerControls;
        _move = temp.Player.Move;
        _move.Enable();

        _roll = temp.Player.Roll;
        _roll.Enable();
        _roll.performed += Roll;

        _run = temp.Player.Run;
        _run.Enable();
    }
    private void OnDisable()
    {
        _move.Disable();
        _roll.Disable();
        _run.Disable();
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();

        _rollCooldown = _rollAnimation.length;
    }

    void FixedUpdate()
    {
        MovementInput();
    }

    #region Moving and Running

    private void MovementInput()
    {
        // Controller
        Vector2 tempMove = _move.ReadValue<Vector2>();

        _movement.Set(tempMove.x, 0f, tempMove.y);
        _movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(_movement.x, 0f);
        bool hasVerticalInput = !Mathf.Approximately(_movement.y, 0f);
        _isMoving = hasHorizontalInput || hasVerticalInput;
        _animator.SetBool("isMoving", _isMoving);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, _movement, _turnSpeed * Time.deltaTime, 0f);
        _rotation = Quaternion.LookRotation(desiredForward);
    }
    private void OnAnimatorMove()
    {
        // Protects against accidental rotation or movement when idling
        if (!_isMoving) return;


        if (_isRolling || _forcedStop) return;

        float currentSpeed = _baseSpeed;
        if (_run.IsPressed())
        {
            currentSpeed = _runSpeed;
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }

        _rb.MovePosition(_rb.position + currentSpeed * Time.deltaTime * _movement);
        _rb.MoveRotation(_rotation);
    }
    private bool _forcedStop = false;
    public void ForceStop(float forThisLong)
    {
        _forcedStop = true;
        Invoke(nameof(UnforceStop), forThisLong - (forThisLong / 5));
    }

    public void UnforceStop()
    {
        _forcedStop = false;
    }
    #endregion

    #region Rolling

    // TODO: Make sure it feels the same when jogging and running
    private void AddForwardForce()
    {
        _rb.AddForce(transform.forward * _rollForce, ForceMode.Impulse);
    }
    public void RollResetAnimEvent()
    {
        _canRoll = true;
        _isRolling = false;
        _animator.ResetTrigger("roll");
    }
    public void RollAnimEvent()
    {
        _isRolling = true;
        AddForwardForce();
    }
    private void Roll(InputAction.CallbackContext context)
    {
        if (!_isMoving || !_canRoll) return;
        _canRoll = false;
        _animator.SetTrigger("roll");
    }
    #endregion
}
