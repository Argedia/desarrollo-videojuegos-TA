using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Health target = other.GetComponent<Health>();
        if (target != null)
        {
            target.TakeDamage(1);
        }
    }
}