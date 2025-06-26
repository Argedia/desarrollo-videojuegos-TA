using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyType> enemyLibrary;
    public Transform enemyParent;
    private int enemiesAlive;

    private List<EnemyType> plannedEnemies = new();

    private void Awake()
    {
        plannedEnemies = new List<EnemyType>();
    }
    private void OnEnable()
    {
        GameEvents.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDeath -= HandleEnemyDeath;
    }
        private void HandleEnemyDeath(Health enemy)
    {
        OnEnemyDeath();
    }
    // Paso 1: planear enemigos y contar los que necesitan plataforma
    public int PlanEnemies(int points)
    {
        plannedEnemies.Clear();
        int groundedCount = 0;
        int attempts = 0;
        Debug.Log("hola enemigos");
        while (points > 0 && attempts < 100)
        {
            var enemy = enemyLibrary[Random.Range(0, enemyLibrary.Count)];
            if (enemy.cost > points) { attempts++; continue; }

            plannedEnemies.Add(enemy);
            if (enemy.requiresPlatform)
                groundedCount++;

            points -= enemy.cost;
            attempts++;
        }

        return groundedCount;
    }

    // Paso 2: instanciar enemigos ya planeados
    public void SpawnEnemies(List<PlatformInstance> platforms)
    {
        foreach (var enemy in plannedEnemies)
        {
            if (!enemy.requiresPlatform)
            {
                Vector3 pos = GetFreeAirPosition(); // define esto tú
                Instantiate(enemy.prefab, pos, Quaternion.identity, enemyParent);
                enemiesAlive++;
                continue;
            }

            var plat = platforms.FirstOrDefault(p => p.remainingSpawns > 0);
            if (plat != null)
            {
                Vector3 spawnPos = plat.GetRandomSpawnPos();
                Instantiate(enemy.prefab, spawnPos, Quaternion.identity, enemyParent);
                plat.remainingSpawns--;
                enemiesAlive++;
            }
            else
            {
                Debug.LogWarning("No hay suficiente suelo para todos los enemigos.");
            }
        }
    }

    public void OnEnemyDeath()
    {
        enemiesAlive--;
        Debug.Log("Quedan vivos:"+ enemiesAlive);
    }
    public bool AllEnemiesDefeated() => enemiesAlive <= 0;

    private Vector3 GetFreeAirPosition()
{
    // Por defecto, genera un punto aleatorio arriba del escenario
    float x = Random.Range(-10f, 10f);
    float y = 5f; // Altura de enemigos voladores
    return new Vector3(x, y, 0f);
}

}
