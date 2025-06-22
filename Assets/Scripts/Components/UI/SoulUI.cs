using UnityEngine;
using UnityEngine.UI;

public class SoulUIController : MonoBehaviour
{
    [SerializeField] private EnergyManager energyManager;
    [SerializeField] private Image soulImage;                 // El componente Image del UI
    [SerializeField] private Sprite[] soulSprites;            // Sprites de 0 a 5 almas (6 sprites)

    private void OnEnable()
    {
        if (energyManager != null)
            energyManager.OnEnergyChanged += UpdateUI;
    }

    private void OnDisable()
    {
        if (energyManager != null)
            energyManager.OnEnergyChanged -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        int current = Mathf.Clamp(energyManager.CurrentEnergy, 0, soulSprites.Length - 1);

        if (soulImage != null && soulSprites != null && soulSprites.Length > 0)
            soulImage.sprite = soulSprites[current];
    }
}
