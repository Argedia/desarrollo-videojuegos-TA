using UnityEngine;

[RequireComponent(typeof(Attacker))]
public class AutoAttacker : MonoBehaviour
{
    [SerializeField] private float detectionRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Vector2 offset = Vector2.zero;

    private Attacker attacker;
    private float cooldownTimer = 0f;
    private Transform playerTransform;

    private void Awake()
    {
        attacker = GetComponent<Attacker>();
        playerTransform = transform;
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f && IsEnemyInFront())
        {
            attacker.Attack();
            cooldownTimer = attackCooldown;
        }
    }

    private bool IsEnemyInFront()
    {
        Vector2 origin = (Vector2)playerTransform.position + offset;
        Vector2 direction = playerTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, detectionRange, enemyLayer);

        if (hit.collider != null)
        {
            Debug.DrawLine(origin, origin + direction * detectionRange, Color.red);
            return true;
        }

        Debug.DrawLine(origin, origin + direction * detectionRange, Color.green);
        return false;
    }
}
