using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BloodVisual))]
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
    private BloodVisual bloodEffect;
    private SoulVisual soulEffect;
    [Header("Blood Effect Settings")]
    public float maxBlood = 50f;
    private void Awake()
    {
        currentHealth = maxHealth;
        if (animator == null)
            animator = GetComponent<Animator>();

        if (bloodEffect == null)
            bloodEffect = GetComponent<BloodVisual>();

        soulEffect = GetComponent<SoulVisual>();
        if (soulEffect == null)
            soulEffect = gameObject.AddComponent<SoulVisual>();

        bloodEffect.CreateDefaultBloodEffect();

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

        UpdateBloodEffect();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            onDeath?.Invoke();

            if (CompareTag("Enemy"))
                GameEvents.EnemyDied(this);

            var controller = GetComponent<IController>();
            controller?.DisableInput();
            animator?.SetTrigger("death");


            bloodEffect?.SetBloodLevel(0);
            soulEffect?.PlaySoulEffect();

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
        UpdateBloodEffect();
    }

    /// <summary>
    /// Fully restores health.
    /// </summary>
    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        UpdateBloodEffect();
    }

    public bool IsDead => currentHealth <= 0;

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(gameObject);
    }
    
    private void UpdateBloodEffect()
    {
        if (bloodEffect == null)
        {
            Debug.Log("Aqui NO hay SANGRE!");
        }

        float bloodLevel = Mathf.Lerp(0, maxBlood, 1f - (float)currentHealth / maxHealth);
        Debug.Log("Nuevo nivel de sangre:"+bloodLevel);
        
        bloodEffect.SetBloodLevel(bloodLevel);
    }
}
