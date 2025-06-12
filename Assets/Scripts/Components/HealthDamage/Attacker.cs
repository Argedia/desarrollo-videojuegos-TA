using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Attacker : MonoBehaviour
{
    Animator anim;
    [SerializeField] private DamageDealer damageDealer;
    private bool canAttack = true;
    private float attackCooldown = 0.2f;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Attack()
    {
        if (!canAttack) return;
        anim.SetTrigger("attack");
        
    }
    // Llamado desde la animación
    public void OnAttackHit()
    {
        damageDealer.TryDealDamage();
        StartCoroutine(AttackCooldownRoutine());
    }

    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
