using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Card
{
    public Sprite sprite;
    public int cost;
}

public class CardManager : MonoBehaviour
{
    [Header("Configuración del mazo")]
    public List<Card> deck = new List<Card>();

    [Header("Energía")]
    public int energyTotal = 10;
    private int energyActual;

    [Header("UI")]
    public Image leftCardImage;
    public Image rightCardImage;

    public TextMeshProUGUI leftCardCost;
    public TextMeshProUGUI rightCardCost;
    public TextMeshProUGUI energyText;

    private void Start()
    {
        energyActual = energyTotal;
        UpdateUI();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            UseCard(0); // Left card
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            UseCard(1); // Right card
        }
    }

    public void UseCard(int index)
    {

        Card selectedCard = deck[index];

        if (energyActual < selectedCard.cost)
        {
            Debug.Log("No hay suficiente energía.");
            return;
        }

        // Gasta energía
        energyActual -= selectedCard.cost;

        // Intercambia la carta usada con la del slot derecho (índice 2)
        Card used = deck[index];
        deck[index] = deck[2];
        deck[2] = used;

        // Elimina la carta usada del índice 2
        deck.RemoveAt(2);

        // La agrega al final del mazo
        deck.Add(used);

        // Actualiza UI
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (deck.Count >= 2)
        {
            leftCardImage.sprite = deck[0].sprite;
            leftCardImage.color = Color.white;
            leftCardCost.text = deck[0].cost.ToString();

            rightCardImage.sprite = deck[1].sprite;
            rightCardImage.color = Color.white;
            rightCardCost.text = deck[1].cost.ToString();
        }

        energyText.text = $"{energyActual}/{energyTotal}";
    }
}
