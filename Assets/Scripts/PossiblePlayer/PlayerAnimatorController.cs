using System;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    // Param hash caching
    private static readonly int IsMovingParam = Animator.StringToHash("isMoving");
    private static readonly int IsGroundedParam = Animator.StringToHash("isGrounded");
    private static readonly int VerticalVelocityParam = Animator.StringToHash("yVelocity");
    private static readonly int IsFlippedParam = Animator.StringToHash("isFlipped");
    private static readonly int AttackTrigger = Animator.StringToHash("attack");
    private static readonly int HitTrigger = Animator.StringToHash("hit");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Set individual animator parameters
    public void SetIsMoving(bool isMoving)
    {
        animator.SetBool(IsMovingParam, isMoving);
    }

    public void SetIsGrounded(bool isGrounded)
    {
        animator.SetBool(IsGroundedParam, isGrounded);
    }

    public void SetVerticalVelocity(float yVelocity)
    {
        animator.SetFloat(VerticalVelocityParam, yVelocity);
    }

    public void SetFacing(float horizontalInput)
    {
        if (horizontalInput > 0f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(AttackTrigger);
    }

    public void TriggerHit()
    {
        animator.SetTrigger(HitTrigger);
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
