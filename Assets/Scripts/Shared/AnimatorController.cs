using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    protected static readonly int IsMovingParam = Animator.StringToHash("isMoving");
    protected static readonly int AttackTrigger = Animator.StringToHash("attack");
    protected static readonly int HitTrigger = Animator.StringToHash("hit");
    protected static readonly int DeathTrigger = Animator.StringToHash("death");

    protected virtual void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void SetIsMoving(bool isMoving)
    {
        animator.SetBool(IsMovingParam, isMoving);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(AttackTrigger);
    }

    public void TriggerHit()
    {
        animator.SetTrigger(HitTrigger);
    }

    public void TriggerDeath()
    {
        animator.SetTrigger(DeathTrigger);
    }

    public void ResetAttackTrigger()
    {
        animator.ResetTrigger(AttackTrigger);
    }

    public void ResetHitTrigger()
    {
        animator.ResetTrigger(HitTrigger);
    }
}
