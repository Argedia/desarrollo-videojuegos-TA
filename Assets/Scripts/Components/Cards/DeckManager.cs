
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; // Si usas Image

public class DeckManager : MonoBehaviour
{
    public List<CardData> drawPile = new();
    public List<CardData> discardPile = new();

    [Header("UI Elements")]
    public Text cardCountText;
    public Image deckBackImage;

    void Start()
    {
        ShuffleDeck();
        UpdateDeckUI();
    }

    public void ShuffleDeck()
    {
        drawPile = drawPile.OrderBy(x => Random.value).ToList();
        UpdateDeckUI();
    }

    public CardData DrawCard()
    {
        Debug.Log("card drawed!");
        if (drawPile.Count == 0)
        {
            if (discardPile.Count == 0)
            {
                UpdateDeckUI();
                return null;
            }

            drawPile = new List<CardData>(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }

        var card = drawPile[0];
        drawPile.RemoveAt(0);
        UpdateDeckUI();
        return card;
    }

    public void Discard(CardData card)
    {
        discardPile.Add(card);
        UpdateDeckUI(); // Si decides mostrar también descarte
    }

    void UpdateDeckUI()
    {
        if (cardCountText != null)
        {
            cardCountText.text = drawPile.Count.ToString();
        }

        if (deckBackImage != null)
        {
            deckBackImage.enabled = drawPile.Count > 0;
        }
    }
}
