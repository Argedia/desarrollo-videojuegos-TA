using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Configuración de Salud")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Eventos")]
    public UnityEvent<int> onDamage; // Enviar daño recibido
    public UnityEvent<int> onHeal;   // Enviar cantidad curada
    public UnityEvent onDeath;

    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        onDamage?.Invoke(amount);

        if (IsDead)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;

        int prevHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        int healedAmount = currentHealth - prevHealth;

        if (healedAmount > 0)
            onHeal?.Invoke(healedAmount);
    }

    private void Die()
    {
        onDeath?.Invoke();
        // Puedes reemplazar esto por una animación o lógica de respawn
        gameObject.SetActive(false);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}
