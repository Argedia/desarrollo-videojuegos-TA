using UnityEngine;

public class Health : MonoBehaviour
{
    public int currentHealth = 10;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isInvulnerable = false;

    private PlayerMovement movement; // o EnemyMovement si reutilizas

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>(); // Cambia por tu script de movimiento si no es PlayerMovement
    }

    public void TakeDamage(int amount, Vector2 knockback)
    {
        if (isInvulnerable) return;

        currentHealth -= amount;
        animator?.SetTrigger("Hurt");
        animator?.SetInteger("Health", currentHealth);

        Debug.Log($"{gameObject.name} took {amount} damage!");

        // Congela movimiento temporalmente
        if (movement != null)
        {
            movement.enabled = false;
        }

        // Knockback
        if (rb != null && knockback != Vector2.zero)
        {
            rb.velocity = Vector2.zero; // Reiniciar antes
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }

        // Rehabilitar movimiento luego de un breve periodo
        Invoke(nameof(RecoverFromHit), 0.4f); // o lo que dure tu animación de daño

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void RecoverFromHit()
    {
        if (movement != null)
        {
            movement.enabled = true;
        }
    }

    private void Die()
    {
        animator?.SetTrigger("Death");
        Debug.Log($"{gameObject.name} died.");

        if (movement != null)
            movement.enabled = false;

        if (rb != null)
            rb.velocity = Vector2.zero;

        // Aquí puedes desactivar colisiones o eliminar el objeto luego de animación
        // Destroy(gameObject, 1.5f);
    }
}
