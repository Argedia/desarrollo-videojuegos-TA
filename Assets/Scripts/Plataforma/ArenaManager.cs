using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public PlatformManager platformManager;

    void StartArena()
    {
        platformManager.SpawnInitialPlatforms();
        platformManager.DisablePlatform(1); // Desactiva una plataforma opcionalmente
    }

    // Puedes llamarlo desde otro script o incluso usar Start() si quieres que se llame al iniciar la escena
    void Start()
    {
        StartArena();
    }
}
