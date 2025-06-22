using System.Collections;
using UnityEngine;

public enum EffectType
{
    Timed,
    Instant,
    LimitedUse
}

[System.Serializable]
public class Effect
{
    public StatType statAffected;
    public float amount;
    public float duration;
    public EffectType effectType;

    public void ApplyTo(GameObject target)
    {
        var provider = target.GetComponent<StatProvider>();
        if (provider == null) return;

        switch (effectType)
        {
            case EffectType.Instant:
                provider.ModifyStat(statAffected, amount);
                break;

            case EffectType.Timed:
                provider.StartCoroutine(ApplyTimedEffect(provider));
                break;

            // Para usar con sistema de input o tracking
            case EffectType.LimitedUse:
                provider.ModifyStat(statAffected, amount);
                // Algún sistema externo debe quitarlo luego de x usos
                break;
        }
    }

    private IEnumerator ApplyTimedEffect(StatProvider provider)
    {
        provider.ModifyStat(statAffected, amount);
        yield return new WaitForSeconds(duration);
        provider.ModifyStat(statAffected, -amount);
    }
}
