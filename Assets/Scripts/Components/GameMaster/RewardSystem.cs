using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    [SerializeField] private EnergyManager energyManager;
    [SerializeField] private int soulsPerKill = 1;

    private void OnEnable()
    {
        GameEvents.OnEnemyDeath += GiveSoul;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDeath -= GiveSoul;
    }

    private void GiveSoul(Health enemy)
    {
        Debug.Log("Alma recaudada");
        if (energyManager != null)
            energyManager.Energy += soulsPerKill;
    }
}
