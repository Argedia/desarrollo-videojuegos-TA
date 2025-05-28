using UnityEngine;

public class DamageDealerOld : MonoBehaviour
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
            HealthOld targetHealth = other.GetComponentInParent<HealthOld>();
            if (targetHealth != null)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
                targetHealth.TakeDamage(damageAmount, pos2D);

                // OPTIONAL: if you only want to hit once, disable
                // isActive = false;
            }
        }
    }
}
