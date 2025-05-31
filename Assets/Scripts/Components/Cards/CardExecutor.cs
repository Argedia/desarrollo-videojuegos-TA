using UnityEngine;

public class CardExecutor : MonoBehaviour
{
    public void ExecuteCard(CardData card, GameObject caster, GameObject target = null, Vector2? direction = null)
    {
        foreach (var effect in card.effects)
        {
            effect.Apply(caster, target, direction);
        }

        if (card.visualEffectPrefab)
        {
            Instantiate(card.visualEffectPrefab, caster.transform.position, Quaternion.identity);
        }
    }
}