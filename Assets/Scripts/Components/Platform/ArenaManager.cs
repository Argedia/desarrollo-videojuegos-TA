using System.Collections;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public PlatformManagerLegacy platformManagerLegacy;
    private int platformCounter = 1;

    // Puedes llamarlo desde otro script o incluso usar Start() si quieres que se llame al iniciar la escena
    void Start()
    {
        StartArena();
        StartCoroutine(SetPlatforms());
    }


    void StartArena()
    {
        platformManagerLegacy.SpawnInitialPlatforms();
        //platformManagerLegacy.DisablePlatform(1); // Desactiva una plataforma opcionalmente
    }

    private IEnumerator SetPlatforms()
    {
        if (platformCounter < platformManagerLegacy.allPlatforms.Count)
        {
            platformManagerLegacy.OnWave(platformCounter);
            platformCounter++;
            Debug.Log(platformCounter);
        }
        else { yield break; }
        yield return new WaitForSeconds(1);
        StartCoroutine(SetPlatforms());
    }
}
