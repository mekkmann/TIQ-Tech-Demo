using UnityEngine;


// DON'T ALTER ANYTHING IN StateBase APART FROM HELPER METHODS
public class StateBase
{
    public enum STATE { IDLE, PURSUIT, ATTACK, DEAD }
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

    // HELPER METHODS
    protected bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.position - npc.transform.position;
        float angleToPlayer = Vector3.Angle(directionToPlayer, npc.transform.forward);

        if (directionToPlayer.magnitude < npc.AggroDistance && angleToPlayer < npc.VisionAngle) return true;

        return false;

    }
    protected bool IsAnimationFinished(string animationName, int animationLayerIndex = 0)
    {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayerIndex);

        return animState.IsName(animationName) && animState.normalizedTime >= 1f;
    }

    protected bool IsAnimationPlaying(string animationName, int animationLayerIndex = 0)
    {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animationLayerIndex);
        return animState.IsName(animationName);
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
        animator.SetBool("isDead", false);
        base.Enter();
    }

    // No updates for Dead-state

    // No exit for Dead-state
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

        // Condition to go to Attack from Idle
        //if (npc.IsTargetInAcceptableDistance())
        //{
        //    nextState = new Attack(npc, animator, player);
        //    stage = EVENT.EXIT;
        //}
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
        // What we always want to do in Pursuit
        npc.MoveToEntity(player);

        // Condition to go Idle from Pursuit
        if (npc.IsTargetInAcceptableDistance())
        {
            nextState = new Idle(npc, animator, player);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        animator.SetBool("isMoving", false);
        base.Exit();
    }
}
public class Attack : StateBase
{

    public Attack(FSMEnemy npc, Animator animator, Transform player) : base(npc, animator, player)
    {
        name = STATE.ATTACK;
    }

    public override void Enter()
    {
        animator.SetTrigger("lightAttack");
        npc.MakeBusy();
        base.Enter();
    }
    public override void Update()
    {
        // TODO: Implement attack
        //if (!npc.IsBusyBool)
        //{
        //    nextState = new Idle(npc, animator, player);
        //    stage = EVENT.EXIT;
        //}
    }
    public override void Exit()
    {
        animator.ResetTrigger("lightAttack");
        base.Exit();
    }
}
