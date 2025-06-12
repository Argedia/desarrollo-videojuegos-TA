using UnityEngine;

public class EnemyPlayerDetector : MonoBehaviour
{
    [Tooltip("Rango máximo en el que se puede detectar al jugador")]
    public float detectionRadius = 5f;

    [Tooltip("Referencia al jugador. Si está vacío, se buscará por tag 'Player'.")]
    public Transform player;

    /// <summary>Devuelve la distancia actual al jugador (infinita si no existe)</summary>
    public float DistanciaAlJugador => player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

    /// <summary>¿El jugador está dentro del radio de detección?</summary>
    public bool JugadorDetectado => DistanciaAlJugador <= detectionRadius;

    void Start()
    {
        if (!player)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found)
            {
                Debug.Log("Player encontrado!");
                player = found.transform;
            }
        }
    }

#if UNITY_EDITOR
    // Gizmo opcional para visualizar el rango en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif
}
