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
        Vector2 spawnPosition = (Vector2)caster.position + relativeSpawnPosition;

        GameObject box = Object.Instantiate(effectBoxPrefab, spawnPosition, Quaternion.identity);
        
        var effectBox = box.GetComponent<EffectBox>();
        if (effectBox != null)
        {
            effectBox.Initialize(effects, caster); // Nuevo método para setear data
            float maxDuration = GetMaxEffectDuration();
            Object.Destroy(box, maxDuration + 0.5f);
        }
        else
        {
            Debug.LogWarning("EffectBox prefab no tiene componente EffectBox.");
        }
    }

    private float GetMaxEffectDuration()
    {
        float max = 0f;
        foreach (var e in effects)
        {
            if (e.duration > max)
                max = e.duration;
        }
        return max;
    }
}
