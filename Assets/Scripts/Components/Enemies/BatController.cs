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
[RequireComponent(typeof(Attacker))]
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
    [SerializeField] float maxDistanceFromPlayer = 6f; 
    [SerializeField] float playerTrackingStrength = 0.3f; // Qué tan fuerte es la atracción hacia el jugador (0.1-1.0)
    
    [Header("Comportamiento de ataque")]
    [SerializeField] float diveChance = 0.3f; // 30% chance per detection
    [SerializeField] float diveCooldown = 3f;
    [SerializeField] float diveDetectionRange = 8f;
    [SerializeField] float returnToPatrolDistance = 12f;
    
    // --- Referencias a los componentes auxiliares ---
    UniformHorizontalMovement horizontalMover;
    UniformVerticalMovement verticalMover;
    EnemyPlayerDetector detector;
    Attacker attacker;
    Rigidbody2D rb; // Referencia al Rigidbody2D
    
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
        attacker = GetComponent<Attacker>();
        rb = GetComponent<Rigidbody2D>();
        
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
        // Ajustar dinámicamente el centro de patrulla hacia el jugador
        UpdatePatrolCenterTowardPlayer();
        
        // Verificar si estamos demasiado lejos del jugador y corregir si es necesario
        if (IsTooFarFromPlayer())
        {
            GenerateNewPatrolTarget(); // Generar nuevo objetivo más cerca del jugador
        }
        
        // Crear movimiento ondulante combinando el objetivo de patrulla con una onda vertical
        float waveOffset = Mathf.Sin(patrolTimer * waveFrequency) * waveAmplitude;
        Vector2 finalTarget = currentPatrolTarget + Vector2.up * waveOffset;
        
        // Un solo movimiento hacia el objetivo con onda aplicada
        MoveTowardsTarget(finalTarget, patrolSpeed);
        
        patrolTimer += Time.deltaTime;
        
        // Si llegamos cerca del objetivo, generar uno nuevo
        if (Vector2.Distance(transform.position, currentPatrolTarget) < 0.8f)
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
        
        // Si llegamos cerca del objetivo, ejecutar ataque
        if (Vector2.Distance(transform.position, diveTarget) < 1.5f)
        {
            // Atacar cuando realmente llega al objetivo
            if (attacker != null)
                attacker.Attack();
            
            state = State.Returning;
            return;
        }
        
        // Si el jugador se alejó mucho, volver a patrullar sin atacar
        if (!detector.JugadorDetectado || 
            detector.DistanciaAlJugador > returnToPatrolDistance)
        {
            state = State.Returning;
        }
    }
    
    void DoReturn()
    {
        // Si estamos demasiado lejos del jugador, priorizar acercarse al jugador
        if (IsTooFarFromPlayer() && detector.player != null)
        {
            Vector2 playerPosition = (Vector2)detector.player.position;
            Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized;
            Vector2 targetNearPlayer = (Vector2)transform.position + directionToPlayer * (maxDistanceFromPlayer * 0.7f);
            
            MoveTowardsTarget(targetNearPlayer, patrolSpeed);
            
            // Si llegamos a una distancia aceptable del jugador, volver a patrullar
            if (!IsTooFarFromPlayer())
            {
                state = State.Patrolling;
                GenerateNewPatrolTarget();
            }
        }
        else
        {
            // Comportamiento normal: regresar hacia el centro de patrullaje
            MoveTowardsTarget(patrolCenter, patrolSpeed);
            
            // Si llegamos cerca del centro, volver a patrullar
            if (Vector2.Distance(transform.position, patrolCenter) < 2f)
            {
                state = State.Patrolling;
                GenerateNewPatrolTarget();
            }
        }
    }
    
    // -------------------------------------------------
    // MÉTODOS PARA MANTENER AL MURCIÉLAGO CERCA DEL JUGADOR
    // -------------------------------------------------
    
    /// <summary>
    /// Ajusta dinámicamente el centro de patrulla hacia la posición del jugador
    /// </summary>
    void UpdatePatrolCenterTowardPlayer()
    {
        if (detector.player == null) return;
        
        Vector2 playerPosition = (Vector2)detector.player.position;
        Vector2 directionToPlayer = (playerPosition - patrolCenter).normalized;
        
        // Mover gradualmente el centro de patrulla hacia el jugador
        patrolCenter = Vector2.MoveTowards(patrolCenter, playerPosition, 
            playerTrackingStrength * patrolSpeed * Time.deltaTime);
        
        // Asegurarse de que el centro no se aleje demasiado de la posición inicial del murciélago
        // (opcional: para evitar que se vaya demasiado lejos de su área original)
        Vector2 initialPosition = (Vector2)transform.position; // Podrías guardar esto en Awake si prefieres
        float maxCenterDistance = maxDistanceFromPlayer * 0.5f; // El centro puede moverse hasta la mitad de la distancia máxima
        
        if (Vector2.Distance(patrolCenter, initialPosition) > maxCenterDistance)
        {
            patrolCenter = Vector2.MoveTowards(patrolCenter, initialPosition, 
                playerTrackingStrength * patrolSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Verifica si el murciélago está demasiado lejos del jugador
    /// </summary>
    bool IsTooFarFromPlayer()
    {
        if (detector.player == null) return false;
        
        float distanceToPlayer = Vector2.Distance(transform.position, (Vector2)detector.player.position);
        return distanceToPlayer > maxDistanceFromPlayer;
    }
    
    // -------------------------------------------------
    void MoveTowardsTarget(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target);
        
        // Suavizar el movimiento cuando está cerca del objetivo
        float speedMultiplier = Mathf.Clamp01(distance / 2f); // Reduce speed within 2 units
        float adjustedSpeed = speed * speedMultiplier;
        
        // Actualizar facing direction solo cuando hay movimiento significativo horizontal
        if (Mathf.Abs(direction.x) > 0.3f)
        {
            if (direction.x > 0 && facingDir != 1)
                Flip();
            else if (direction.x < 0 && facingDir != -1)
                Flip();
        }
        
        // Mover suavemente en ambas direcciones
        horizontalMover.Move(direction.x * speedMultiplier);
        verticalMover.Move(direction.y * speedMultiplier);
    }
    
    void GenerateNewPatrolTarget()
    {
        // Si estamos demasiado lejos del jugador, generar objetivo hacia el jugador
        if (IsTooFarFromPlayer() && detector.player != null)
        {
            Vector2 playerPosition = (Vector2)detector.player.position;
            Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized;
            
            // Generar objetivo a una distancia intermedia hacia el jugador
            float targetDistance = maxDistanceFromPlayer * 0.6f; // 60% de la distancia máxima
            currentPatrolTarget = (Vector2)transform.position + directionToPlayer * targetDistance;
            return;
        }
        
        // Comportamiento normal de patrulla
        // Generar un punto aleatorio dentro del radio de patrulla
        // Favorecer puntos más alejados del centro para vuelos más naturales
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(patrolRadius * 0.6f, patrolRadius);
        Vector2 randomOffset = randomDirection * randomDistance;
        Vector2 proposedTarget = patrolCenter + randomOffset;
        
        // Verificar que el objetivo propuesto no esté demasiado lejos del jugador
        if (detector.player != null)
        {
            float distanceFromPlayerToTarget = Vector2.Distance(proposedTarget, (Vector2)detector.player.position);
            if (distanceFromPlayerToTarget > maxDistanceFromPlayer)
            {
                // Si está muy lejos, ajustar el objetivo hacia el jugador
                Vector2 directionToPlayer = ((Vector2)detector.player.position - proposedTarget).normalized;
                float excessDistance = distanceFromPlayerToTarget - maxDistanceFromPlayer;
                proposedTarget += directionToPlayer * (excessDistance + 1f); // +1f para estar seguro dentro del rango
            }
        }
        
        currentPatrolTarget = proposedTarget;
        
        // Asegurar que el objetivo no esté demasiado cerca de la posición actual
        int attempts = 0;
        while (Vector2.Distance(transform.position, currentPatrolTarget) < 1.5f && attempts < 5)
        {
            randomDirection = Random.insideUnitCircle.normalized;
            randomDistance = Random.Range(patrolRadius * 0.6f, patrolRadius);
            randomOffset = randomDirection * randomDistance;
            currentPatrolTarget = patrolCenter + randomOffset;
            attempts++;
        }
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
        
        // Mensaje para efectos de sonido/visuales del inicio del dive
        SendMessage("OnBatStartDive", SendMessageOptions.DontRequireReceiver);
    }

    // --- MÉTODOS DE MUERTE Y CAÍDA ---
    /// <summary>
    /// Llama este método cuando el murciélago muere (por ejemplo, desde DamageReceiver o un Animation Event).
    /// </summary>
    public void OnDeathFall()
    {
        // Desactivar la IA y el input
        DisableInput();
        // Desactivar scripts de movimiento
        if (horizontalMover) horizontalMover.enabled = false;
        if (verticalMover) verticalMover.enabled = false;
        // Habilitar gravedad y caída
        if (rb)
        {
            rb.gravityScale = 2.5f; // Ajusta según lo rápido que quieras que caiga
            rb.linearVelocity = Vector2.zero; // Detener cualquier movimiento previo
            rb.constraints = RigidbodyConstraints2D.None; // Permitir rotación si quieres que caiga girando
        }
        if (attacker) attacker.enabled = false;
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
        
        // Dibujar la distancia máxima al jugador
        if (Application.isPlaying && detector != null && detector.player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detector.player.position, maxDistanceFromPlayer);
            
            // Dibujar línea de distancia actual al jugador
            float currentDistance = Vector2.Distance(transform.position, detector.player.position);
            Gizmos.color = currentDistance > maxDistanceFromPlayer ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, detector.player.position);
        }
        
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
