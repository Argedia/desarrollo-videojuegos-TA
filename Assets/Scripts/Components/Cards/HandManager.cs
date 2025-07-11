using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject cardUIPrefab;
    public Transform handPanel;
    public DeckManager deckManager;
    private int maxHandSize = 4;

    private CardUI[] currentHand;  // Usamos un arreglo fijo para las cartas.

    void Start()
    {
        currentHand = new CardUI[maxHandSize];  // Inicializamos el arreglo con el tamaño máximo.
        FillHand();
    }

    public void FillHand()
    {
        if (deckManager.DeckSize < maxHandSize)
        {
            Debug.Log("Tamaño de Deck es muy pequeño para llenar la mano");
            return;
        }
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard(i);
        }
    }

    void DrawCard(int index)
    {   
        
        if (index >= maxHandSize) return;  // Asegurarnos de no exceder el tamaño máximo.

        Debug.Log("card requested!!");
        CardData newCard = deckManager.DrawCard();
        if (newCard == null) return;

        Transform slot = handPanel.GetChild(index);
        // Si ya hay una carta en este slot, solo actualizamos el CardUI.
        if (currentHand[index] == null)
        {
            // Si el slot está vacío, obtenemos el CardUI de este slot
            currentHand[index] = slot.GetComponent<CardUI>();
        }

        // Configuramos la carta con los datos de la carta robada.
        currentHand[index].Setup(newCard);  // Configuramos la carta con los datos.
    }


    public void TryUseCard(int index, Transform caster, ref int currentEnergy)
    {
        if (index < 0 || index >= maxHandSize) return;

        // Usar la carta.
        CardUI card = currentHand[index];
        CardData cardData = card.GetData();
        if (cardData.CanPlayCard(currentEnergy))
        {
            currentEnergy-=cardData.energyCost;
            cardData.Activate(caster);
            deckManager.Discard(card.GetData());
            // Marca la carta como eliminada en el arreglo.
            currentHand[index] = null;
            DrawCard(index);
        }
        else
        {
            //Agregar visuales aqui
            Debug.Log("No tienes suficiente energía para usar esta carta.");
        }
    }
}
