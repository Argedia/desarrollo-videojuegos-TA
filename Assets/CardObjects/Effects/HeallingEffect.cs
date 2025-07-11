using UnityEngine;

public class HealingEffect : MonoBehaviour, IEffect
{
    public int healAmmount = 1;

    public void ApplyTo(GameObject target)
    {
        if (target.layer == LayerMask.NameToLayer("Enemies"))
            return;
        var runner = target.GetComponent<MonoBehaviour>();
        if (runner != null)
            runner.StartCoroutine(Heal(target));
    }

    private System.Collections.IEnumerator Heal(GameObject target)
    {

        var health = target.GetComponent<Health>() ?? target.GetComponentInParent<Health>();
        if (health != null)
            Debug.Log("Te estan curando papito");
        var sprite = target.GetComponent<SpriteRenderer>();
        Color originalColor = sprite != null ? sprite.color : Color.white;

        if (sprite != null)
            sprite.color = Color.green; // 💚 Indicar visualmente curación

        health.Heal(healAmmount);
        yield return new WaitForSeconds(healAmmount/10);

        if (sprite != null)
            sprite.color = originalColor; // 🔄 Volver al color original
    }
}