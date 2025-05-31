
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardCollection : MonoBehaviour
{
    public List<CardData> unlockedCards = new(); // Solo las que el jugador desbloqueó

    public bool HasUnlocked(CardData card)
    {
        return unlockedCards.Contains(card);
    }

    public void UnlockCard(CardData card)
    {
        if (!unlockedCards.Contains(card))
            unlockedCards.Add(card);
    }
}
