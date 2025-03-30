using UnityEngine;


public class FSMEnemy : MonoBehaviour
{
    private enum ActionState { IDLE, MOVING, ATTACKING };

    [Header("FSM")]
    [SerializeField] private ActionState _actionState;
    [SerializeField] private bool _stateCooldown = false;
    [SerializeField] private float _acceptableDistanceToTarget = 5;
    [SerializeField] private bool _reposition = false;
    [SerializeField] private float _currentDistanceToTarget;
    [SerializeField] private float _aggroDistance = 20f;
    [SerializeField] private float _visionAngle = 60f;
    [SerializeField] private string _targetTag = "Player";
    [SerializeField] private GameObject _target;
    [SerializeField] public AnimationClip AttackAnimation;

    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _turnSpeed = 4f;

    public float AcceptableDistanceToTarget => _acceptableDistanceToTarget;
    public bool StateCooldown => _stateCooldown;
    public float VisionAngle => _visionAngle;
    public float AggroDistance => _aggroDistance;


    [SerializeField] private bool _isBusy = false;
    public bool IsBusyBool => _isBusy;

    private Animator _animator;
    private Rigidbody _rigidBody;

    private StateBase _currentState;

    public StateBase State => _currentState;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _target = GameObject.FindGameObjectWithTag(_targetTag);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentState = new Idle(this, _animator, _target.transform);
    }

    // Update is called once per frame
    void Update()
    {
        _currentState = _currentState.Process();
    }
    public void HandleIsBusy(bool isBusy)
    {
        _isBusy = isBusy;
        _animator.SetBool("isBusy", isBusy);
        Debug.Log("IS: " + _isBusy);
    }

    public void NotBusy()
    {
        HandleIsBusy(false);
    }
    public void MakeBusy()
    {
        HandleIsBusy(true);
    }

    private float DistanceToEntity(Transform entity) => Vector3.Distance(entity.position, transform.position);
    private void MoveAwayFromEntity(Transform entity)
    {
        if (entity == null) return;

        Vector3 directionAwayFromEntity = DirectionToEntity(entity, opposite: true);

        transform.rotation = Quaternion.LookRotation(directionAwayFromEntity);
        MoveForward(_moveSpeed);
    }
    private bool TargetNotNull() => _target != null;

    public void MoveToEntity(GameObject target)
    {
        MoveToEntity(target.transform);
    }
    public void MoveToEntity(Transform entity)
    {
        if (_isBusy) return;

        MoveForward(_moveSpeed);
        LookAt(entity);
    }
    private void MoveForward(float speed) => transform.position += speed * Time.deltaTime * transform.forward;
    private Vector3 DirectionToEntity(Transform entity, bool opposite = false)
    {
        if (entity == null) return Vector3.zero;

        Vector3 directionToEntity = entity.position - transform.position;
        directionToEntity.Normalize();

        return directionToEntity * (opposite ? -1 : 1);
    }

    public void LookAt(Vector3 position) => transform.LookAt(position.With(y: transform.position.y));
    public void LookAt(Transform entity) => transform.LookAt(entity.position.With(y: transform.position.y));

    public bool IsTargetInAggroDistance() => DistanceToEntity(_target.transform) <= _aggroDistance;
    public bool IsTargetInAcceptableDistance() => DistanceToEntity(_target.transform) <= _acceptableDistanceToTarget;
    private bool TryGetTarget()
    {
        GameObject temp = GameObject.FindGameObjectWithTag(_targetTag);
        if (temp != null)
        {
            if (Vector3.Distance(temp.transform.position, transform.position) <= _aggroDistance)
            {
                _target = temp;
                return true;
            }
        }

        return false;
    }

    private Vector3 Cross(Vector3 v, Vector3 w)
    {
        float x = v.y * w.z - v.z * w.y;
        float y = v.x * w.z - v.z * w.x;
        float z = v.x * w.y - v.y * w.x;

        Debug.Log($"Cross X = {x}\nCross Y = {y}\nCross z = {z}");

        return new(x, y, z);
    }
    private void OnDrawGizmos()
    {
        // Display aggro range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _aggroDistance);

        // Display acceptable distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _acceptableDistanceToTarget);
    }
}
