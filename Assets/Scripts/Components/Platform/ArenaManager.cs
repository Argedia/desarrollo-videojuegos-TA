// using System.Collections;
// using UnityEngine;

// public class ArenaManager : MonoBehaviour
// {
//     public PlatformManager platformManager;
//     private int platformCounter = 1;

//     // Puedes llamarlo desde otro script o incluso usar Start() si quieres que se llame al iniciar la escena
//     void Start()
//     {
//         StartArena();
//         StartCoroutine(SetPlatforms());
//     }
    
    
//     void StartArena()
//     {
//         platformManager.SpawnInitialPlatforms();
//         //platformManager.DisablePlatform(1); // Desactiva una plataforma opcionalmente
//     }

//     private IEnumerator SetPlatforms()
//     {
//         if (platformCounter < platformManager.allPlatforms.Count)
//         {
//             platformManager.OnWave(platformCounter);
//             platformCounter++;
//             Debug.Log(platformCounter);
//         }
//         else{ yield break;}
//         yield return new WaitForSeconds(1);
//         StartCoroutine(SetPlatforms());
//     }
// }
