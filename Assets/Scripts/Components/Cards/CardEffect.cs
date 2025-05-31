using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void Apply(GameObject caster, GameObject target = null, Vector2? direction = null);
}
