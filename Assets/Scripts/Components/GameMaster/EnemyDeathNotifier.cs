using UnityEngine;

/// <summary>
/// Componente que conecta los eventos de muerte de los enemigos con el EnemyManager
/// Adjunta este script a cada enemigo o úsalo como referencia para configurar los eventos
/// </summary>
public class EnemyDeathNotifier : MonoBehaviour
{
    private Health healthComponent;
    private EnemyManager enemyManager;

    void Start()
    {
        // Obtener el componente Health del enemigo
        healthComponent = GetComponent<Health>();
        
        // Encontrar el EnemyManager en la escena
        enemyManager = FindFirstObjectByType<EnemyManager>();
        
        if (healthComponent != null && enemyManager != null)
        {
            // Conectar el evento de muerte con el EnemyManager
            healthComponent.onDeath.AddListener(OnEnemyDeath);
            Debug.Log($"EnemyDeathNotifier: Connected death event for {gameObject.name}");
        }
        else
        {
            if (healthComponent == null)
                Debug.LogError($"EnemyDeathNotifier: No Health component found on {gameObject.name}");
            if (enemyManager == null)
                Debug.LogError($"EnemyDeathNotifier: No EnemyManager found in scene");
        }
    }

    private void OnEnemyDeath()
    {
        Debug.Log($"EnemyDeathNotifier: {gameObject.name} has died, notifying EnemyManager");
        enemyManager.OnEnemyDeath();
    }

    void OnDestroy()
    {
        // Desconectar el evento al destruir el objeto
        if (healthComponent != null && enemyManager != null)
        {
            healthComponent.onDeath.RemoveListener(OnEnemyDeath);
        }
    }
}
