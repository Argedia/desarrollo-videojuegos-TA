using UnityEngine;

public class CardPickup : MonoBehaviour
{
    public CardData cardData; // Asignado cuando se instancia

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DeckManager deck = FindObjectOfType<DeckManager>();
            if (deck != null && cardData != null)
            {
                deck.drawPile.Add(cardData);
                deck.ShuffleDeck(); // Opcional
                Debug.Log($"¡Agregaste {cardData.cardName} a tu mazo!");

                // Destruir otras cartas en escena
                foreach (var pickup in FindObjectsOfType<CardPickup>())
                {
                    if (pickup != this)
                        Destroy(pickup.gameObject);
                }

                Destroy(gameObject);
            }
        }
    }
}
