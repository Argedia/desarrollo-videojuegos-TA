using System;
using System.Collections;
using UnityEngine;

public enum WaveState
{
    Idle,
    Spawning,
    InProgress,
    Resting,
    Completed,
    Failed
}

public class WaveManager : MonoBehaviour
{
    public void Awake()
    {
        Start();
    }
    public PlatformManager platformManager;
    public EnemyManager enemyManager;

    public float waveTime = 30f;
    public float restTime = 5f;

    public WaveState State { get; private set; } = WaveState.Idle;
    public int CurrentWave { get; private set; } = 1;

    // Eventos
    public static event Action<int> OnWaveStarted;
    public static event Action<int> OnWaveCompleted;
    public static event Action<int> OnRestStarted;
    public static event Action OnWaveFailed;
    public static event Action OnAllWavesCompleted;

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        while (true)
        {
            State = WaveState.Spawning;
            
            int platformPoints = GetPlatformPoints(CurrentWave);
            Debug.Log("puntos plataforma:" + platformPoints);
            int enemyPoints = GetEnemyPoints(CurrentWave);
            Debug.Log("puntos enemigos:" + enemyPoints);
            int groundedEnemies = enemyManager.PlanEnemies(enemyPoints);
            Debug.Log("ayuda");
            platformManager.GeneratePlatforms(platformPoints, groundedEnemies);
            enemyManager.SpawnEnemies(platformManager.GetSpawnablePlatforms());

            OnWaveStarted?.Invoke(CurrentWave);
            State = WaveState.InProgress;
            float timer = waveTime;

            while (State == WaveState.InProgress)
            {
                timer -= Time.deltaTime;

                if (enemyManager.AllEnemiesDefeated())
                {
                    State = WaveState.Resting;
                    OnWaveCompleted?.Invoke(CurrentWave);
                    OnRestStarted?.Invoke(CurrentWave);

                    yield return new WaitForSeconds(restTime);
                    CurrentWave++;
                    break;
                }

                if (timer <= 0f)
                {
                    State = WaveState.Failed;
                    OnWaveFailed?.Invoke();
                    Debug.Log("Game Over");
                    yield break;
                }

                yield return null;
            }

            // Aquí puedes agregar una condición para terminar el juego
            if (CurrentWave > 10)
            {
                State = WaveState.Completed;
                OnAllWavesCompleted?.Invoke();
                break;
            }
        }
    }

    private int GetPlatformPoints(int w) => 2 + w * 2;
    private int GetEnemyPoints(int w) => 0 + w * 3;
}
