using UnityEngine;
public class Health : MonoBehaviour
{
    public int currentHealth = 10;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int amount, Vector2 knockback)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage!");

        animator?.SetTrigger("Hurt");

        // Apply knockback if needed
        if (knockback != Vector2.zero)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(knockback, ForceMode2D.Impulse);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        // TODO: death animation, disable controls, etc.
    }
}
