using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyType> enemyLibrary;
    public Transform enemyParent;

    private List<EnemyType> plannedEnemies = new();

        private void Awake()
    {
        plannedEnemies = new List<EnemyType>();
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
        Debug.Log($"EnemyManager: Spawning {plannedEnemies.Count} enemies");
        
        foreach (var enemy in plannedEnemies)
        {
            if (!enemy.requiresPlatform)
            {
                Vector3 pos = GetFreeAirPosition(); // define esto tú
                GameObject spawnedEnemy = Instantiate(enemy.prefab, pos, Quaternion.identity, enemyParent);
                Debug.Log($"EnemyManager: Spawned flying enemy {enemy.name} at {pos}");
                continue;
            }

            var plat = platforms.FirstOrDefault(p => p.remainingSpawns > 0);
            if (plat != null)
            {
                Vector3 spawnPos = plat.GetRandomSpawnPos();
                GameObject spawnedEnemy = Instantiate(enemy.prefab, spawnPos, Quaternion.identity, enemyParent);
                plat.remainingSpawns--;
                Debug.Log($"EnemyManager: Spawned ground enemy {enemy.name} at {spawnPos}");
            }
            else
            {
                Debug.LogWarning("No hay suficiente suelo para todos los enemigos.");
            }
        }
        
        Debug.Log($"EnemyManager: Total alive enemies after spawn: {GetAliveEnemyCount()}");
    }

    // Método para compatibilidad - ya no necesario pero mantenido por si se usa en otros lugares
    public void OnEnemyDeath() 
    {
        Debug.Log($"EnemyManager: Enemy death reported. Alive enemies: {GetAliveEnemyCount()}");
    }
    
    // Nueva implementación robusta basada en Health.IsDead
    public bool AllEnemiesDefeated() 
    {
        int aliveCount = GetAliveEnemyCount();
        Debug.Log($"EnemyManager: AllEnemiesDefeated check - Alive enemies: {aliveCount}");
        return aliveCount <= 0;
    }
    
    // Método auxiliar para contar enemigos vivos usando Health.IsDead
    private int GetAliveEnemyCount()
    {
        int count = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            Health healthComponent = enemy.GetComponent<Health>();
            if (healthComponent != null && !healthComponent.IsDead)
            {
                count++;
            }
            else if (healthComponent == null)
            {
                // Si no tiene componente Health, asumimos que está vivo
                Debug.LogWarning($"Enemy {enemy.name} doesn't have Health component!");
                count++;
            }
        }
        
        return count;
    }

    private Vector3 GetFreeAirPosition()
{
    // Por defecto, genera un punto aleatorio arriba del escenario
    float x = Random.Range(-10f, 10f);
    float y = 5f; // Altura de enemigos voladores
    return new Vector3(x, y, 0f);
}

}
