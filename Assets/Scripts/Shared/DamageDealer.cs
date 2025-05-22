using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;
    public LayerMask targetLayers;
    public bool isActive = true;

    [Header("Optional Knockback")]
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        // Check if the target is in the correct layer
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            // Try to get Health component on the other object
            Health targetHealth = other.GetComponentInParent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damageAmount, knockback);

                // OPTIONAL: if you only want to hit once, disable
                // isActive = false;
            }
        }
    }
}
