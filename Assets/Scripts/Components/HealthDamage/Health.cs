using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 2;
    public int currentHealth = 2;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent onDamaged;

    [SerializeField] private Animator animator;
    [SerializeField] private float timeToDestroy = 2f;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Apply damage. Returns true if damaged.
    /// </summary>
    public bool TakeDamage(int amount)
    {
        if (currentHealth <= 0) return false;

        currentHealth -= amount;
        onDamaged?.Invoke();

        if (CompareTag("Enemy")) // Solo si es enemigo
            GameEvents.EnemyDamaged(this, amount);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            onDeath?.Invoke();

            if (CompareTag("Enemy"))
                GameEvents.EnemyDied(this);

            var controller = GetComponent<IController>();
            controller?.DisableInput();
            animator?.SetTrigger("death");
            StartCoroutine(DelayedDestroy());
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
    
        private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(gameObject);
    }
}
