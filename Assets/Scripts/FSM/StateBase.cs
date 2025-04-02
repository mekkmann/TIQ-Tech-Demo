using UnityEngine;


// DON'T ALTER ANYTHING IN StateBase APART FROM HELPER METHODS
public class StateBase
{
    public enum STATE { IDLE, PURSUIT, LIGHTATTACK, DEAD, BLOCKING }
    public enum EVENT { ENTER, UPDATE, EXIT }

    public STATE name;
    protected EVENT stage;
    protected FSMEnemy npc;
    protected Transform player;
    protected Animator animator;
    protected StateBase nextState;

    public StateBase(FSMEnemy npc, Animator anim, Transform player)
    {
        this.npc = npc;
        this.animator = anim;
        this.player = player;

        stage = EVENT.ENTER;
    }

    public virtual void Enter()
    {
        Debug.Log($"Entering FSM State: [{name}]");

        stage = EVENT.UPDATE;
    }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit()
    {
        Debug.Log($"Exiting FSM State: [{name}]");

        stage = EVENT.EXIT;
    }

    public StateBase Process()
    {
        if (stage == EVENT.ENTER)
        {
            Enter();
        }
        if (stage == EVENT.UPDATE)
        {
            Update();
        }
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
}
// State Implementations
public class Dead : StateBase
{
    public Dead(FSMEnemy npc, Animator anim, Transform player) : base(npc, anim, player)
    {
        name = STATE.DEAD;
    }
    public override void Enter()
    {
        animator.SetBool("isDead", true);
        base.Enter();
    }

    // No override for Update

    // No override for Exit
}
public class Idle : StateBase
{
    public Idle(FSMEnemy npc, Animator anim, Transform player) : base(npc, anim, player)
    {
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        #region What we always want to do in idle
        npc.LookAt(player);
        #endregion

        #region Conditional state changes
        // Condition to go to Pursuit from Idle
        if (!npc.IsTargetInAcceptableDistance())
        {
            nextState = new Pursuit(npc, animator, player);
            stage = EVENT.EXIT;
        }

        // Condition to go to Blocking from Idle (10% success rate)
        if (npc.IsTargetInAcceptableDistance() && Random.Range(0f, 1f) <= 0.1f)
        {
            nextState = new Blocking(npc, animator, player, npc.GetRandomBlockDuration());
            stage = EVENT.EXIT;
        }
        else if (npc.IsTargetInAcceptableDistance()) // Condition to go to Attack from Idle
        {
            nextState = new LightAttack(npc, animator, player, npc.AttackAnimation);
            stage = EVENT.EXIT;
        }

        #endregion
    }

    public override void Exit()
    {
        base.Exit();
    }
}
public class Pursuit : StateBase
{
    public Pursuit(FSMEnemy npc, Animator anim, Transform player) : base(npc, anim, player)
    {
        name = STATE.PURSUIT;
    }

    public override void Enter()
    {
        animator.SetBool("isMoving", true);
        base.Enter();
    }
    public override void Update()
    {
        #region What we always want to do
        // What we always want to do in Pursuit
        npc.MoveToEntity(player);
        #endregion

        #region Conditional state changes
        // Condition to go Idle from Pursuit
        if (npc.IsTargetInAcceptableDistance())
        {
            nextState = new Idle(npc, animator, player);
            stage = EVENT.EXIT;
        }
        #endregion
    }
    public override void Exit()
    {
        animator.SetBool("isMoving", false);
        base.Exit();
    }
}
public class LightAttack : StateBase
{
    float _attackTimer = -1f;
    public LightAttack(FSMEnemy npc, Animator animator, Transform player, AnimationClip animation) : base(npc, animator, player)
    {
        name = STATE.LIGHTATTACK;
        _attackTimer = animation.length;
    }
    public override void Enter()
    {
        animator.SetTrigger("lightAttack");
        base.Enter();
    }
    public override void Update()
    {
        // TODO: Make it clean by finding a RELIABLE way of checking if animation has finished instead of using a timer
        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0)
        {
            nextState = new Idle(npc, animator, player);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        animator.ResetTrigger("lightAttack");
        base.Exit();
    }
}

public class Blocking : StateBase
{
    private float _blockTimer;
    public Blocking(FSMEnemy npc, Animator animator, Transform player, float blockTimer = 1f) : base(npc, animator, player)
    {
        name = STATE.BLOCKING;
        _blockTimer = blockTimer;
    }
    public override void Enter()
    {
        npc.HandleBlocking(true);
        animator.SetBool("isBlocking", true);
        base.Enter();
    }
    public override void Update()
    {
        _blockTimer -= Time.deltaTime;
        if (_blockTimer <= 0)
        {
            nextState = new Idle(npc, animator, player);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        Debug.Log("bish");
        npc.HandleBlocking(false);
        animator.SetBool("isBlocking", false);
        base.Exit();
    }
}
