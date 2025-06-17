using UnityEngine;

public class EffectBox : MonoBehaviour
{
    private Effect[] effects;
    private Transform caster;

    public void Initialize(Effect[] newEffects, Transform origin)
    {
        effects = newEffects;
        caster = origin;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var effect in effects)
        {
            effect.ApplyTo(other.gameObject);
        }
    }
}
