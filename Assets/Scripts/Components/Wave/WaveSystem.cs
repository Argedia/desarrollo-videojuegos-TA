// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public enum WaveState
// {
//     Idle,
//     Starting,
//     InProgress,
//     Resting,
//     Completed,
//     Failed
// }

// public class WaveManager : MonoBehaviour
// {
//     [Header("Configuraci�n de Oleadas")]
//     [SerializeField]
//     private List<WaveData> waves = new List<WaveData>();

//     [Header("Tiempos")]
//     [SerializeField]
//     private float restTime = 5f; // Descanso tras oleada exitosa

//     public int CurrentWave { get; private set; } = 0;
//     public WaveState State { get; private set; } = WaveState.Idle;

//     // Eventos para comunicar
//     public static event Action<int> OnWaveStarted;
//     public static event Action<int> OnWaveEnded;
//     public static event Action<int> OnRestStarted;
//     public static event Action OnAllWavesCompleted;
//     public static event Action OnWaveFailed;

//     private Coroutine waveRoutine;

//     private void Start()
//     {
//         // Iniciar la primera oleada
//         BeginNextWave();
//     }

//     public void BeginNextWave()
//     {
//         if (CurrentWave < waves.Count)
//         {
//             CurrentWave++;
//             waveRoutine = StartCoroutine(RunWave(waves[CurrentWave - 1]));
//         }
//         else
//         {
//             State = WaveState.Completed;
//             OnAllWavesCompleted?.Invoke();
//             Debug.Log("�Todas las oleadas completadas!");
//         }
//     }

//     private IEnumerator RunWave(WaveData data)
//     {
//         State = WaveState.Starting;
//         OnWaveStarted?.Invoke(data.waveNumber);
//         Debug.Log($"Oleada {data.waveNumber} iniciada.");

//         State = WaveState.InProgress;
//         float timer = 0f;
//         int aliveCount = data.enemyPrefabs.Count; // Ejemplo simple

//         // Aqu� llamar�as a EnemiesManager y PlataformaManager
//         EnemiesManager.Instance.SpawnEnemies(data.enemyPrefabs);

//         while (timer < data.timeLimit && !EnemiesManager.Instance.AllEnemiesDefeated)
//         {
//             timer += Time.deltaTime;
//             yield return null;
//         }

//         if (EnemiesManager.Instance.AllEnemiesDefeated)
//         {
//             State = WaveState.Resting;
//             OnWaveEnded?.Invoke(data.waveNumber);
//             Debug.Log($"Oleada {data.waveNumber} completada en {timer:F1}s. Descanso...");

//             OnRestStarted?.Invoke(data.waveNumber);
//             yield return new WaitForSeconds(restTime);

//             BeginNextWave();
//         }
//         else
//         {
//             State = WaveState.Failed;
//             OnWaveFailed?.Invoke();
//             Debug.Log("�Oleada fallida! Tiempo agotado.");
//         }
//     }
// }

// // Ejemplo de PlatformManager escuchando eventos
// public class PlataformaManager : MonoBehaviour
// {
//     private void OnEnable()
//     {
//         WaveManager.OnWaveStarted += HandleWaveStart;
//     }
//     private void OnDisable()
//     {
//         WaveManager.OnWaveStarted -= HandleWaveStart;
//     }

//     private void HandleWaveStart(int waveNumber)
//     {
//         // Mostrar plataformas espec�ficas
//         Debug.Log($"Configurar plataformas para la oleada {waveNumber}");
//         // L�gica para activar/desactivar plataformas seg�n waveNumber
//     }
// }

// // Ejemplo de EnemiesManager (singleton simple)
// public class EnemiesManager : MonoBehaviour
// {
//     public static EnemiesManager Instance { get; private set; }
//     public bool AllEnemiesDefeated { get; private set; }

//     private List<GameObject> activeEnemies = new List<GameObject>();

//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }

//     public void SpawnEnemies(List<GameObject> enemyPrefabs)
//     {
//         activeEnemies.Clear();
//         foreach (var prefab in enemyPrefabs)
//         {
//             GameObject e = Instantiate(prefab);
//             activeEnemies.Add(e);
//         }
//         AllEnemiesDefeated = false;
//         // Suscribir a eventos de muerte de enemigos...
//     }

//     private void Update()
//     {
//         activeEnemies.RemoveAll(e => e == null);
//         if (activeEnemies.Count == 0)
//         {
//             AllEnemiesDefeated = true;
//         }
//     }
// }

