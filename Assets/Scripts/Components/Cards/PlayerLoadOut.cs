using System.Collections.Generic;
using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    public List<CardData> selectedCards = new(); 
    [SerializeField] private int maxCards = 6; // Máx 4 o 6 según sistema

    public void SelectCard(CardData card)
    {
        if (!selectedCards.Contains(card) && selectedCards.Count < maxCards)
            selectedCards.Add(card);
    }

    public void DeselectCard(CardData card)
    {
        selectedCards.Remove(card);
    }

    public void ClearLoadout()
    {
        selectedCards.Clear();
    }
}
