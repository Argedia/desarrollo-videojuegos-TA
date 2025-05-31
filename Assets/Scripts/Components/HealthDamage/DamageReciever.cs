using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageReceiver : MonoBehaviour
{
    private Health health;
    private KnockbackReceiver knockbackReceiver;
    private TemporaryInvulnerability tempInvulnerability;
    private Animator animator;

    [SerializeField]
    private float invulnerabilityDuration = 1f; // Customize per entity in Inspector

    private void Awake()
    {
        health = GetComponent<Health>();
        knockbackReceiver = GetComponent<KnockbackReceiver>();
        tempInvulnerability = GetComponent<TemporaryInvulnerability>();
        animator = GetComponent<Animator>();
    }

    public void ReceiveDamage(int damage,
                              Vector2 hitOrigin,
                              float knockbackForce = 0f)
    {
        if (health.IsDead) return;
        if (tempInvulnerability != null && tempInvulnerability.IsActive) return;

        bool damaged = health.TakeDamage(damage);
        if (damaged)
        {
            tempInvulnerability?.Activate(invulnerabilityDuration);

            knockbackReceiver?.ApplyKnockback(hitOrigin, knockbackForce);
            animator?.SetTrigger("hit");
        }
    }
}

