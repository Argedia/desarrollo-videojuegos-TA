using UnityEngine;

public class platformManagerWave : MonoBehaviour
{
    public PlatformManager platformManager;

    void OnWave(int waveNumber)
    {
        switch (waveNumber)
        {
            case 1:
                break;

            case 2:
                break;

            case 3:
                break;

            default:
                platformManager.ClearAll();
                break;
        }
    }

}
