using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageReceiver : MonoBehaviour
{
    private Health health;
    private KnockbackReceiver knockbackReceiver;
    private TemporaryInvulnerability tempInvulnerability;
    private Animator animator;
    private IController controller;
    private bool shieldActive = false;

    [SerializeField]
    private float invulnerabilityDuration = 1f; // Customize per entity in Inspector

    private void Awake()
    {
        controller = GetComponent<IController>();
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

        if (shieldActive)
        {
            shieldActive = false;
            Debug.Log("🛡 Daño bloqueado por escudo.");
            return;
        }

        bool damaged = health.TakeDamage(damage);
        if (damaged)
        {
            tempInvulnerability?.Activate(invulnerabilityDuration);

            //controller?.DisableInput(); //aquí desactivas el control

            knockbackReceiver?.ApplyKnockback(hitOrigin, knockbackForce);
            animator?.SetTrigger("hit");

            // Vuelve a activar el input después de invulnerabilidad
            if (controller != null)
                StartCoroutine(ReenableControlAfterDelay(invulnerabilityDuration));
        }
    }

    private System.Collections.IEnumerator ReenableControlAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //controller.EnableInput();
    }
    public void ApplyShield()
    {
        shieldActive = true;
    }
    public bool ShieldIsActive => shieldActive;

}

