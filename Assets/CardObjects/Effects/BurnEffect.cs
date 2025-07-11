using UnityEngine;

public class BurnEffect : MonoBehaviour, IEffect
{
    public int damagePerTick = 1;
    public float tickInterval = 0.5f;
    public float duration = 3f;

    public void ApplyTo(GameObject target)
    {
        if (target.layer == LayerMask.NameToLayer("Player"))
            return;
        var runner = target.GetComponent<MonoBehaviour>();
        if (runner != null)
            runner.StartCoroutine(Burn(target));
    }

    private System.Collections.IEnumerator Burn(GameObject target)
    {
        var health = target.GetComponent<Health>();
        var sprite = target.GetComponent<SpriteRenderer>();
        Color originalColor = sprite != null ? sprite.color : Color.white;

        if (sprite != null)
            sprite.color = Color.red;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            health?.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        if (sprite != null)
            sprite.color = originalColor;
    }
}
