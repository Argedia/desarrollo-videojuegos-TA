using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackReceiver : MonoBehaviour
{
    [Header("Knockback Settings")]
    [Tooltip("Default force used if none is provided explicitly.")]
    [SerializeField] private float defaultForce = 10f;
    [SerializeField] private float upwardBoost = 0.5f; // fuerza vertical añadida

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Applies knockback from a hit origin with a given force.
    /// </summary>
    /// <param name="hitOrigin">The position the hit came from.</param>
    /// <param name="force">The force magnitude. If zero or negative, defaultForce is used.</param>
    public void ApplyKnockback(Vector2 hitOrigin, float force = 0f)
    {
        if (rb == null) return;

        float finalForce = (force > 0f) ? force : defaultForce;
        Vector2 direction = (rb.position - hitOrigin).normalized;

        // Vector vertical puro hacia arriba
        Vector2 verticalComponent = Vector2.up * upwardBoost;

        // Mezclar los vectores con ponderación y luego normalizar
        direction = (direction + verticalComponent).normalized;
        
        //rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * finalForce, ForceMode2D.Impulse);
    }
}
