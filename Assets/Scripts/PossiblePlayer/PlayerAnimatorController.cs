using System;
using UnityEngine;

public class PlayerAnimatorController : AnimationController
{
    // Param hash caching
    private static readonly int IsGroundedParam = Animator.StringToHash("isGrounded");
    private static readonly int VerticalVelocityParam = Animator.StringToHash("yVelocity");

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
}
