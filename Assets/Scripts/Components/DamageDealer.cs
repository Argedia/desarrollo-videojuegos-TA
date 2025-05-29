using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private LayerMask damageableLayers;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only try to damage objects in the specified layers
        if (((1 << other.gameObject.layer) & damageableLayers) == 0)
            return;

        DamageReceiver receiver = other.GetComponent<DamageReceiver>();
        if (receiver != null)
        {
            Vector2 hitOrigin = transform.position;
            receiver.ReceiveDamage(damageAmount, hitOrigin, knockbackForce);
        }
    }
}
