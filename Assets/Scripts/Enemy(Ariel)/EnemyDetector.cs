using UnityEngine;
using UnityEngine.Events;

public class EnemyDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayers;
    public Transform origin;

    [Header("Events")]
    public UnityEvent<GameObject> onPlayerDetected;
    public UnityEvent onPlayerLost;

    private GameObject detectedPlayer;

    void Awake()
    {
        origin = transform;
    }
    void Update()
    {
        Collider2D player = Physics2D.OverlapCircle(origin.position, detectionRadius, playerLayer);

        if (player != null)
        {
            Vector2 dirToPlayer = (player.transform.position - origin.position).normalized;
            float distance = Vector2.Distance(origin.position, player.transform.position);

            RaycastHit2D hit = Physics2D.Raycast(origin.position, dirToPlayer, distance, obstacleLayers | playerLayer);

            if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
            {
                if (detectedPlayer == null)
                {
                    detectedPlayer = player.gameObject;
                    onPlayerDetected.Invoke(detectedPlayer);
                }
                return;
            }
        }

        if (detectedPlayer != null)
        {
            detectedPlayer = null;
            onPlayerLost.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (origin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin.position, detectionRadius);
        }
    }

    public GameObject GetDetectedPlayer() => detectedPlayer;
    public bool HasDetectedPlayer => detectedPlayer != null;
}
