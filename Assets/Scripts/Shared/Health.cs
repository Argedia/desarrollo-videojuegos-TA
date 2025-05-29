using UnityEngine;
using System.Collections;
public class HealthOld : MonoBehaviour
{
    public int currentHealth = 10;
    private PlayerAnimatorController animController;
    private bool isInvulnerable = false;

    private PlayerMovement movement; // o EnemyMovement si reutilizas

    private void Awake()
    {
        animController = GetComponentInChildren<PlayerAnimatorController>();
        movement = GetComponent<PlayerMovement>(); // Cambia por tu script de movimiento si no es PlayerMovement
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (isInvulnerable) return;

        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        Vector2 knockback = new Vector2(direction.x, 1f).normalized * 5f; // dirección + impulso vertical

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
            return;
        }
        animController?.TriggerHit();

        Debug.Log($"{gameObject.name} took {amount} damage!");

        // Congela movimiento temporalmente
        if (movement != null)
        {
            movement.ApplyKnockback(knockback);
        }
    }

    private IEnumerator Die()
    {
        movement.Freeze(-1);
        animController?.TriggerDeath();
        Debug.Log($"{gameObject.name} died.");
        //Destruye el objeto luego de morir
        yield return new WaitForSeconds(10);
        Destroy(gameObject, 1.5f);
    } 
}
