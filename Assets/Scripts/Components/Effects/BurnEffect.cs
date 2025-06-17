using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Burn")]
public class BurnEffect : ScriptableObject, IEffect
{
    public int damagePerTick = 1;
    public float tickInterval = 1f;
    public float duration = 3f;
    public Color colorWhileBurning = Color.red;

    public void Apply(GameObject target)
    {
        var runner = target.GetComponent<MonoBehaviour>();
        if (runner != null)
        {
            runner.StartCoroutine(DoBurn(target));
        }
    }

    private IEnumerator DoBurn(GameObject target)
    {
        var health = target.GetComponent<Health>();
        var sr = target.GetComponent<SpriteRenderer>();
        Color original = sr ? sr.color : Color.white;

        if (sr) sr.color = colorWhileBurning;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            health?.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        if (sr) sr.color = original;
    }
}
