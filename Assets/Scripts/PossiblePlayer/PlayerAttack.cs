using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown = 0.5f;

    private bool canAttack = true;
    private bool isAttacking = false;

    private PlayerAnimatorController animator;
    private Coroutine cooldownCoroutine;
    private PlayerMovement movement;

    private void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimatorController>();
        movement = GetComponent<PlayerMovement>();
    }

    public void Attack()
    {
        if (!canAttack || !movement.IsGrounded()) return;

        canAttack = false;
        isAttacking = true;

        animator.TriggerAttack();
        cooldownCoroutine = StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        movement.Freeze();
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;
        movement.FreeMove();
    }

    public void InterruptAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            canAttack = true;
            movement.FreeMove();
            if (cooldownCoroutine != null)
                StopCoroutine(cooldownCoroutine);

            animator.ResetAttackTrigger();
            // Opcional: agregar lógica extra para revertir estado de ataque
        }
    }
}
