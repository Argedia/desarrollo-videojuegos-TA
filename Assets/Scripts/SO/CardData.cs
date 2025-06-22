using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite icon;
    public Effect[] effects;
    public GameObject effectBoxPrefab;
    public Vector2 relativeSpawnPosition;

    public void Activate(Transform caster)
    {
        foreach (var effect in effects)
        {
            Vector2 spawnPosition = (Vector2)caster.position + relativeSpawnPosition;
            var box = Object.Instantiate(effectBoxPrefab, spawnPosition, Quaternion.identity);
            box.GetComponent<EffectBox>().effect = effect;
            Object.Destroy(box, effect.duration + 0.5f);
        }
    }
}
