using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardID;
    public string cardName;
    public Sprite icon;
    [TextArea] public string description;

    public float manaCost;
    public float cooldown;

    public GameObject visualEffectPrefab;

    public CardEffect[] effects; // Muy importante: lista de efectos a aplicar
}