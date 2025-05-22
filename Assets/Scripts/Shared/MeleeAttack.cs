using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public GameObject hitbox; // Asigna tu hijo "AttackHitbox"
    public float attackDuration = 0.2f;
    private bool isAttacking = false;
    [SerializeField] private Animator animator;
    
    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    
    public void TryAttack()
    {
        if (!isAttacking)
            StartCoroutine(PerformAttack());
    }

    System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;
        hitbox.SetActive(true);
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(attackDuration);
        hitbox.SetActive(false);
        isAttacking = false;
    }
}
