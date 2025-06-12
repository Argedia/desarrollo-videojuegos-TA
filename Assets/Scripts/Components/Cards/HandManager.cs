using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject cardUIPrefab;
    public Transform handPanel;
    public DeckManager deckManager;
    public int maxHandSize = 2;

    private List<CardUI> currentHand = new();

    void Start()
    {
        FillHand();
    }

    public void FillHand()
    {
        while (currentHand.Count < maxHandSize)
        {
            DrawCard();
        }
    }

    void DrawCard()
    {
        Debug.Log("card requested!!");
        CardData newCard = deckManager.DrawCard();
        if (newCard == null) return;

        GameObject go = Instantiate(cardUIPrefab, handPanel);
        CardUI cardUI = go.GetComponent<CardUI>();
        cardUI.Setup(newCard);
        currentHand.Add(cardUI);
    }

    public void UseCard(int index, Transform caster)
    {
        if (index < 0 || index >= currentHand.Count) return;

        var card = currentHand[index];
        card.Activate(caster);
        deckManager.Discard(card.GetData());

        currentHand.RemoveAt(index);
        Destroy(card.gameObject);
        DrawCard();
    }
}
