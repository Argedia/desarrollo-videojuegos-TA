using UnityEngine;

/// <summary>
/// Inteligencia básica para un murciélago 2D:
///  • Vuela en patrones circulares/ondulantes por el aire.
///  • Ocasionalmente detecta al jugador y se lanza hacia él en línea recta.
///  • Después de atacar, regresa a su patrón de vuelo libre.
/// </summary>
[RequireComponent(typeof(UniformHorizontalMovement))]
[RequireComponent(typeof(UniformVerticalMovement))]
[RequireComponent(typeof(EnemyPlayerDetector))]
public class BatController : Controller, IController
{
    private bool inputEnabled = true;
    
    // --- Parámetros de balance ---
    [Header("Velocidades")]
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float diveSpeed = 6f;
    
    [Header("Patrón de vuelo")]
    [SerializeField] float waveAmplitude = 2f;
    [SerializeField] float waveFrequency = 1f;
    [SerializeField] float patrolRadius = 4f;
    
    [Header("Comportamiento de ataque")]
    [SerializeField] float diveChance = 0.3f; // 30% chance per detection
    [SerializeField] float diveCooldown = 3f;
    [SerializeField] float diveDetectionRange = 8f;
    [SerializeField] float returnToPatrolDistance = 12f;
    
    // --- Referencias a los componentes auxiliares ---
    UniformHorizontalMovement horizontalMover;
    UniformVerticalMovement verticalMover;
    EnemyPlayerDetector detector;
    Animator animator;
    
    // --- Estado interno ---
    enum State { Patrolling, Diving, Returning }
    State state = State.Patrolling;
    
    // --- Variables de patrullaje ---
    Vector2 patrolCenter;
    float patrolTimer = 0f;
    Vector2 currentPatrolTarget;
    
    // --- Variables de ataque ---
    float lastDiveTime = -10f;
    Vector2 diveTarget;
    
    // -------------------------------------------------
    void Awake()
    {
        horizontalMover = GetComponent<UniformHorizontalMovement>();
        verticalMover = GetComponent<UniformVerticalMovement>();
        detector = GetComponent<EnemyPlayerDetector>();
        animator = GetComponent<Animator>();
        
        // El centro de patrullaje es la posición inicial
        patrolCenter = transform.position;
        GenerateNewPatrolTarget();
    }
    
    void Start()
    {
        // Configurar detección personalizada para el rango de dive
        detector.detectionRadius = diveDetectionRange;
    }
    
    void Update()
    {
        if (!inputEnabled) return;
        
        switch (state)
        {
            case State.Patrolling:
                DoPatrol();
                break;
            case State.Diving:
                DoDive();
                break;
            case State.Returning:
                DoReturn();
                break;
        }
    }
    
    // -------------------------------------------------
    void DoPatrol()
    {
        // Movimiento ondulante hacia el objetivo de patrulla
        MoveTowardsTarget(currentPatrolTarget, patrolSpeed);
        
        // Añadir movimiento ondulante vertical
        float waveOffset = Mathf.Sin(patrolTimer * waveFrequency) * waveAmplitude;
        Vector2 waveTarget = currentPatrolTarget + Vector2.up * waveOffset;
        MoveTowardsTarget(waveTarget, patrolSpeed);
        
        patrolTimer += Time.deltaTime;
        
        // Si llegamos cerca del objetivo, generar uno nuevo
        if (Vector2.Distance(transform.position, currentPatrolTarget) < 1f)
        {
            GenerateNewPatrolTarget();
        }
        
        // ¿Detectamos al jugador y podemos atacar?
        if (detector.JugadorDetectado && CanDive())
        {
            if (Random.value < diveChance * Time.deltaTime) // Chance por segundo
            {
                StartDive();
            }
        }
    }
    
    void DoDive()
    {
        // Movimiento directo hacia el objetivo de dive (donde estaba el jugador)
        MoveTowardsTarget(diveTarget, diveSpeed);
        
        // Si el jugador se alejó mucho, volver a patrullar
        if (!detector.JugadorDetectado || 
            detector.DistanciaAlJugador > returnToPatrolDistance)
        {
            state = State.Returning;
        }
        
        // Si llegamos cerca del objetivo, regresar
        if (Vector2.Distance(transform.position, diveTarget) < 1.5f)
        {
            state = State.Returning;
        }
    }
    
    void DoReturn()
    {
        // Regresar hacia el centro de patrullaje
        MoveTowardsTarget(patrolCenter, patrolSpeed);
        
        // Si llegamos cerca del centro, volver a patrullar
        if (Vector2.Distance(transform.position, patrolCenter) < 2f)
        {
            state = State.Patrolling;
            GenerateNewPatrolTarget();
        }
    }
    
    // -------------------------------------------------
    void MoveTowardsTarget(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        
        // Actualizar facing direction
        if (direction.x > 0.1f && facingDir != 1)
            Flip();
        else if (direction.x < -0.1f && facingDir != -1)
            Flip();
        
        // Mover en ambas direcciones
        horizontalMover.Move(direction.x);
        verticalMover.Move(direction.y);
    }
    
    void GenerateNewPatrolTarget()
    {
        // Generar un punto aleatorio dentro del radio de patrulla
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        currentPatrolTarget = patrolCenter + randomOffset;
    }
    
    bool CanDive()
    {
        return Time.time - lastDiveTime >= diveCooldown;
    }
    
    void StartDive()
    {
        state = State.Diving;
        diveTarget = detector.player.position;
        lastDiveTime = Time.time;
        animator.SetTrigger("Attack");
        SendMessage("OnBatDive", SendMessageOptions.DontRequireReceiver);
        
        // Enviar mensaje de ataque para efectos/sonidos
        SendMessage("OnBatDive", SendMessageOptions.DontRequireReceiver);
    }

    public void Die()
    {
        DisableInput();
        animator.SetTrigger("Die");
        // Desactivar colisión, etc.
    }

    // Llamado cuando recibe daño
    public void TakeHit()
    {
        animator.SetTrigger("Hit");
    }
    
    // -------------------------------------------------
    public void DisableInput()
    {
        inputEnabled = false;
        horizontalMover.Move(0f);
        verticalMover.Move(0f);
    }
    
    public void EnableInput()
    {
        inputEnabled = true;
    }

    
    // -------------------------------------------------
    // Gizmos para visualizar el comportamiento en el editor
    void OnDrawGizmosSelected()
    {
        // Dibujar el área de patrullaje
        Gizmos.color = Color.cyan;
        Vector3 center = Application.isPlaying ? patrolCenter : transform.position;
        Gizmos.DrawWireSphere(center, patrolRadius);
        
        // Dibujar el objetivo actual de patrulla
        if (Application.isPlaying && state == State.Patrolling)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentPatrolTarget, 0.5f);
            Gizmos.DrawLine(transform.position, currentPatrolTarget);
        }
        
        // Dibujar el objetivo de dive
        if (Application.isPlaying && state == State.Diving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(diveTarget, 0.5f);
            Gizmos.DrawLine(transform.position, diveTarget);
        }
    }
}
