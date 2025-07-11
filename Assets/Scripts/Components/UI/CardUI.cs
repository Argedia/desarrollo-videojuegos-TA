using System;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public CardData data;
    public Image iconImage;
    public Text energyText;
    public CardData GetData() => data;

    public void Setup(CardData newData)
    {
        data = newData;
        Debug.Log(data.icon.name);
        iconImage.sprite = data.icon;
        energyText.text = data.energyCost.ToString();
    }
}
