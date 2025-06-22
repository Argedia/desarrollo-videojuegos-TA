using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int currentEnergy;
    public int maxEnergy = 10;

    public int CurrentEnergy { get { return currentEnergy; } }
    // Propiedad Energy con getter y setter
    public int Energy
    {
        get => currentEnergy;
        set
        {
            // Asegurarse de que la energía esté dentro del rango permitido
            currentEnergy = Mathf.Clamp(value, 0, maxEnergy);

            // Disparar evento para notificar cambios
            OnEnergyChanged?.Invoke();
        }
    }

    // Evento que se dispara cuando la energía cambia
    public event System.Action OnEnergyChanged;

    // Método para verificar si el jugador tiene suficiente energía para una carta
    public bool CanAffordEnergy(int energyCost)
    {
        return Energy >= energyCost;
    }

    // Método para gastar energía
    public void UseEnergy(int energyCost)
    {
        if (CanAffordEnergy(energyCost))
        {
            Energy -= energyCost;
        }
    }
}
