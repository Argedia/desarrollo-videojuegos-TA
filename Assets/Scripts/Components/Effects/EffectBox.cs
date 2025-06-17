using System.Collections.Generic;
using UnityEngine;

public class EffectBox : MonoBehaviour
{
    [SerializeField]
    private List<ScriptableObject> effects;

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var effectObj in effects)
        {
            if (effectObj is IEffect effect)
            {
                effect.Apply(other.gameObject);
            }
        }
    }
}
