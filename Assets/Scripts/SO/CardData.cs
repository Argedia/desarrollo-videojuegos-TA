using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    [Header("Información básica")]
    public string cardName;
    public Sprite icon;

    [Header("Prefab que representa la carta jugada")]
    [Tooltip("Prefab que contiene visuales, animaciones, colisionadores y lógica del efecto")]
    public GameObject effectObjectPrefab;

    [Tooltip("Relativo al jugador. Útil para spawn delante o sobre él")]
    public Vector2 relativeSpawnPosition;

    [Header("Costo de energía")]
    [Tooltip("Cuánta energía consume jugar esta carta")]
    public int energyCost;

    /// <summary>
    /// Verifica si el jugador tiene suficiente energía para jugar esta carta.
    /// </summary>
    public bool CanPlayCard(int currentEnergy)
    {
        return currentEnergy >= energyCost;
    }

    /// <summary>
    /// Activa la carta, instanciando su EffectObject prefab y pasándole el caster.
    /// </summary>
    public void Activate(Transform caster)
    {
        if (effectObjectPrefab == null)
        {
            Debug.LogWarning($"La carta '{cardName}' no tiene asignado un effectObjectPrefab.");
            return;
        }

        Vector2 spawnPosition = (Vector2)caster.position + relativeSpawnPosition;

        GameObject go = Instantiate(effectObjectPrefab, spawnPosition, Quaternion.identity);

        var obj = go.GetComponent<EffectObject>();
        obj.Setup(caster);
        // var effectObject = go.GetComponent<EffectObject>();
        // if (effectObject != null)
        // {
        //     effectObject.Activate(caster);
        // }
        // else
        // {
        //     Debug.LogWarning($"El prefab '{effectObjectPrefab.name}' no tiene un componente EffectObject.");
        // }
    }
}
