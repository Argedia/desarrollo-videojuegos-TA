using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent onDamaged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Apply damage. Returns true if damaged.
    /// </summary>
    public bool TakeDamage(int amount)
    {
        if (currentHealth <= 0) return false;

        currentHealth -= amount;
        onDamaged?.Invoke();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            onDeath?.Invoke();
        }

        return true;
    }

    /// <summary>
    /// Heals the character up to maxHealth.
    /// </summary>
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    /// <summary>
    /// Fully restores health.
    /// </summary>
    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
    }

    public bool IsDead => currentHealth <= 0;
}
