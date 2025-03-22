using UnityEngine;
using UnityEngine.AI;

public enum ActionState { IDLE, WORKING };
public class Enemy : Character
{
    private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject _target;

    private ActionState _actionState = ActionState.IDLE;

    private BehaviorTree _wanderAndFindTargetBT;
    private Status _wanderAndFindTargetStatus = Status.RUNNING;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _wanderAndFindTargetBT = new("Wander and Find [BehaviorTree]");

        Sequence wanderAndFind = new("Wander and Find [Sequence]");

        Leaf lookForTarget = new("Look for Target", LookForTarget);
        wanderAndFind.AddChild(lookForTarget);
        Leaf wander = new("Go To Random Location [Leaf]", Wander);
        wanderAndFind.AddChild(wander);


        _wanderAndFindTargetBT.AddChild(wanderAndFind);

        _wanderAndFindTargetBT.PrintTree();
    }

    [Range(0, 100)]
    [SerializeField] private float _visualDetectionRange = 100;
    private Status LookForTarget()
    {
        if (_target == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (Vector3.Distance(playerGO.transform.position, transform.position) <= _visualDetectionRange)
            {
                _target = playerGO;
                Debug.Log("TARGET FOUND BREAK BEHAVIOR TREE");
                return Status.FAILURE;
            }
        }

        Debug.Log("TARGET NOT FOUND CONTINUE TO WANDER");
        return Status.SUCCESS;
    }

    private Vector3 _wanderTarget = Vector3.zero;
    private Status Wander()
    {
        return GoToLocation(GetRandomPosition());

    }

    private Vector3 GetRandomPosition()
    {
        float wanderRadius = 5f;
        float wanderDistance = 5f;
        float wanderJitter = 1f;

        _wanderTarget += new Vector3(Random.Range(-1f, 1f) * wanderJitter, 0, Random.Range(-1f, 1f) * wanderJitter);

        _wanderTarget.Normalize();
        _wanderTarget *= wanderRadius;

        Vector3 targetLocal = _wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal);

        return targetWorld;
    }
    private Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, transform.position);
        if (_actionState == ActionState.IDLE)
        {
            _navMeshAgent.SetDestination(destination);
            _actionState = ActionState.WORKING;
        }
        else if (Vector3.Distance(_navMeshAgent.pathEndPosition, destination) >= 2)
        {
            Debug.Log("Previous End Pos: " + _navMeshAgent.pathEndPosition);
            Debug.Log("Previous Destination: " + destination);
            Debug.Log(Vector3.Distance(_navMeshAgent.pathEndPosition, destination));
            _actionState = ActionState.IDLE;
            return Status.FAILURE;
        }
        else if (distanceToTarget <= 2)
        {
            _actionState = ActionState.IDLE;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
        {
            _wanderAndFindTargetStatus = _wanderAndFindTargetBT.Process();

        }

    }
}
