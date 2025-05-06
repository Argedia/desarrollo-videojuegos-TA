using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public Image card1Image, card2Image;
    public TMP_Text energyText;
    public TMP_Text card1CostText;
    public TMP_Text card2CostText;

    private int currentEnergy = 20;
    private int maxEnergy = 20;
    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            UseCard(0);

        if (Input.GetKeyDown(KeyCode.C))
            UseCard(1);
    }

    void UseCard(int index)
    {
        if (deck.Count < 2) return;

        Card selectedCard = deck[index];
        if (currentEnergy >= selectedCard.cost)
        {
            currentEnergy -= selectedCard.cost;
            Card used = deck[index];
            deck[index] = deck[2];
            deck.Add(used);
            deck.RemoveAt(2);
            UpdateUI();
        }
        else
        {
            Debug.Log("No suficiente energía");
        }
    }

    void UpdateUI()
    {
        card1Image.sprite = deck[0].icon;
        card2Image.sprite = deck[1].icon;
        card1CostText.text = deck[0].cost.ToString();
        card2CostText.text = deck[1].cost.ToString();
        energyText.text = currentEnergy + "/" + maxEnergy;
    }
}

