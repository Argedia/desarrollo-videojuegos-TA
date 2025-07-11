using UnityEngine;

/// <summary>
/// Inteligencia para un Hongo Corredor 2D:
///  • Patrulla en línea recta sobre la plataforma.
///  • Al detectar al jugador, se lanza corriendo con alta velocidad.
///  • Alterna entre carrera rápida y saltos poderosos para sorprender al jugador.
///  • Puede saltar por encima de obstáculos pequeños para perseguir agresivamente.
/// </summary>
[RequireComponent(typeof(UniformHorizontalMovement))]
[RequireComponent(typeof(EdgeDetector))]
[RequireComponent(typeof(EnemyPlayerDetector))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Jump))]
public class MushroomController : Controller, IController
{
    private bool inputEnabled = true;
    
    // --- Parámetros de balance ---
    [Header("Velocidades")]
    [SerializeField] float patrolSpeed = 1.5f;
    [SerializeField] float rushSpeed = 6f; // Velocidad de carrera rápida hacia el jugador
    [SerializeField] float chaseSpeed = 3f;

    [Header("Rangos")]
    [SerializeField] float attackRange = 1.2f;
    [SerializeField] float rushDetectionRange = 8f; // Rango para iniciar carrera rápida
    
    [Header("Comportamiento de Salto")]
    [SerializeField] float jumpChance = 0.4f; // 40% chance de saltar durante la carrera
    [SerializeField] float maxJumpDistance = 5f; // Distancia máxima para intentar saltar hacia el jugador

    // --- Referencias a los componentes auxiliares ---
    UniformHorizontalMovement mover;
    EdgeDetector edge;
    EnemyPlayerDetector detector;
    Rigidbody2D rb;
    Jump jumpComponent; // Nuevo: Referencia al componente Jump

    // --- Estado interno ---
    enum State { Patrolling, Chasing, Rushing, Jumping, Attacking, Dead }
    State state = State.Patrolling;
    Animator animator;

    // -------------------------------------------------
    void Awake()
    {
        mover = GetComponent<UniformHorizontalMovement>();
        edge = GetComponent<EdgeDetector>();
        detector = GetComponent<EnemyPlayerDetector>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jumpComponent = GetComponent<Jump>(); // Obtener referencia al componente Jump
        
        // Configurar el componente Jump con nuestros parámetros
        if (jumpComponent != null)
        {
            jumpComponent.ConfigureJump(12f, 2f); // jumpForce, jumpCooldown
            
            // Suscribirse a eventos del componente Jump
            jumpComponent.OnJumpStarted += OnJumpStarted;
            jumpComponent.OnLanded += OnLanded;
        }
    }

    void Start()
    {
        // Garantiza que el movimiento arranque en la dirección inicial.
        mover.Move(facingDir);
    }

    void Update()
    {
        if (!inputEnabled || state == State.Dead) return;

        switch (state)
        {
            case State.Patrolling:
                DoPatrol();
                break;
            case State.Chasing:
                DoChase();
                break;
            case State.Rushing:
                DoRush();
                break;
            case State.Jumping:
                DoJump();
                break;
            case State.Attacking:
                DoAttack();
                break;
            case State.Dead:
                // No hacer nada, está muerto
                break;
        }
    }

    // -------------------------------------------------
    void DoPatrol()
    {
        // Cambiar de borde → invertimos dirección
        if (!edge.isGroundAhead)
            Flip();

        mover.Move(facingDir);
        animator.SetBool("isMoving", true);

        //¿Vio al jugador? Si está cerca, iniciar carrera rápida
        if (detector.JugadorDetectado)
        {
            if (detector.DistanciaAlJugador <= rushDetectionRange)
            {
                state = State.Rushing; // Carrera rápida y agresiva
                animator.SetBool("isRushing", true);
            }
            else
            {
                state = State.Chasing; // Persecución normal
            }
        }
    }

    void DoChase()
    {
        // Elegimos la dirección hacia el jugador
        int dirToPlayer = detector.player.position.x > transform.position.x ? 1 : -1;
        animator.SetBool("isMoving", dirToPlayer != 0);
        
        // Si el jugador está cerca, cambiar a carrera rápida
        if (detector.DistanciaAlJugador <= rushDetectionRange)
        {
            state = State.Rushing;
            animator.SetBool("isRushing", true);
            return;
        }
        
        // Si no hay suelo delante y seguiríamos cayendo, nos detenemos
        if (!edge.isGroundAhead && dirToPlayer == facingDir)
        {
            mover.Move(0);   // se frena en el borde
            return;
        }

        // Si la dirección cambió con respecto al "facingDir", flip visual
        if (dirToPlayer != facingDir)
            Flip();

        mover.Move(facingDir);

        // ¿Está suficientemente cerca para atacar?
        if (detector.DistanciaAlJugador <= attackRange)
        {
            mover.Move(0);
            state = State.Attacking;
            animator.SetBool("isMoving", false);
        }
        // ¿Perdió al jugador?
        else if (!detector.JugadorDetectado)
        {
            state = State.Patrolling;
            animator.SetBool("isMoving", true);
            mover.Move(facingDir);
        }
    }

    void DoRush()
    {
        // Elegimos la dirección hacia el jugador
        int dirToPlayer = detector.player.position.x > transform.position.x ? 1 : -1;
        animator.SetBool("isMoving", true);
        animator.SetBool("isRushing", true);
        
        // Verificar si puede saltar hacia el jugador usando el componente Jump
        if (CanJumpToPlayer() && ShouldJumpToPlayer())
        {
            TryJumpToPlayer();
            return;
        }
        
        // Si no hay suelo delante, intentar saltar en lugar de detenerse
        if (!edge.isGroundAhead && dirToPlayer == facingDir)
        {
            if (CanJumpToPlayer() && detector.DistanciaAlJugador <= maxJumpDistance)
            {
                TryJumpToPlayer();
                return;
            }
            else
            {
                // Si no puede saltar, detenerse en el borde
                mover.Move(0);
                state = State.Chasing; // Volver a persecución normal
                animator.SetBool("isRushing", false);
                return;
            }
        }

        // Si la dirección cambió, hacer flip
        if (dirToPlayer != facingDir)
            Flip();

        // Moverse con velocidad de carrera rápida
        mover.Move(facingDir * (rushSpeed / chaseSpeed)); // Multiplicador de velocidad

        // ¿Está suficientemente cerca para atacar?
        if (detector.DistanciaAlJugador <= attackRange)
        {
            mover.Move(0);
            state = State.Attacking;
            animator.SetBool("isMoving", false);
            animator.SetBool("isRushing", false);
        }
        // ¿Se alejó mucho el jugador? Volver a persecución normal
        else if (detector.DistanciaAlJugador > rushDetectionRange * 1.5f)
        {
            state = State.Chasing;
            animator.SetBool("isRushing", false);
        }
        // ¿Perdió al jugador?
        else if (!detector.JugadorDetectado)
        {
            state = State.Patrolling;
            animator.SetBool("isMoving", true);
            animator.SetBool("isRushing", false);
            mover.Move(facingDir);
        }
    }

    void DoJump()
    {
        // Durante el salto, mantener dirección hacia el jugador en el aire
        if (detector.JugadorDetectado)
        {
            int dirToPlayer = detector.player.position.x > transform.position.x ? 1 : -1;
            
            // Pequeño control aéreo hacia el jugador
            if (dirToPlayer != facingDir)
                Flip();
                
            // Aplicar un poco de movimiento horizontal en el aire
            mover.Move(facingDir * 0.7f);
        }
        
        // Si aterriza (el componente Jump maneja esto automáticamente)
        if (jumpComponent.IsGrounded && !jumpComponent.IsJumping)
        {
            // El evento OnLanded se encargará de cambiar el estado
        }
    }

    void DoAttack()
    {
        // Aquí no nos movemos: golpea otro script que escuche este evento.
        // Ejemplo: BroadcastMessage, UnityEvent, etc.
        animator.SetTrigger("Attack"); 
        animator.SetBool("isMoving", false);
        animator.SetBool("isRushing", false);
        SendMessage("OnMushroomAttack", SendMessageOptions.DontRequireReceiver);

        // Si el jugador se aleja, volvemos a persecución o patrulla
        if (detector.DistanciaAlJugador > attackRange * 1.3f)
        {
            if (detector.JugadorDetectado)
            {
                // Decidir entre rush o chase basado en la distancia
                if (detector.DistanciaAlJugador <= rushDetectionRange)
                    state = State.Rushing;
                else
                    state = State.Chasing;
            }
            else
            {
                state = State.Patrolling;
            }
        }
    }

    // -------------------------------------------------
    // MÉTODOS OPTIMIZADOS USANDO EL COMPONENTE JUMP
    // -------------------------------------------------
    
    /// <summary>
    /// Verifica si puede saltar usando el componente Jump
    /// </summary>
    bool CanJumpToPlayer()
    {
        return jumpComponent != null && jumpComponent.CanJump;
    }
    
    /// <summary>
    /// Determina si debería saltar hacia el jugador basado en probabilidad y distancia
    /// </summary>
    bool ShouldJumpToPlayer()
    {
        if (!detector.JugadorDetectado) return false;
        
        float playerDistance = detector.DistanciaAlJugador;
        
        // Solo saltar si el jugador está a una distancia razonable
        if (playerDistance > maxJumpDistance || playerDistance < attackRange) 
            return false;
            
        // Probabilidad de salto basada en la cercanía del jugador
        float adjustedJumpChance = jumpChance;
        if (playerDistance < maxJumpDistance * 0.5f)
            adjustedJumpChance *= 1.5f; // Más probabilidad si está más cerca
            
        return Random.value < adjustedJumpChance * Time.deltaTime;
    }
    
    /// <summary>
    /// Ejecuta el salto hacia el jugador usando el componente Jump
    /// </summary>
    void TryJumpToPlayer()
    {
        if (!CanJumpToPlayer() || !detector.JugadorDetectado) return;
        
        Vector2 playerPosition = detector.player.position;
        
        // Usar el método del componente Jump para saltar hacia el jugador
        if (jumpComponent.TryJumpToPosition(playerPosition, 0.7f))
        {
            state = State.Jumping;
            
            // Orientar hacia el jugador
            int dirToPlayer = playerPosition.x > transform.position.x ? 1 : -1;
            if (dirToPlayer != facingDir)
                Flip();
                
            // Mensaje para efectos de sonido/visuales
            SendMessage("OnMushroomJump", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    // -------------------------------------------------
    // EVENTOS DEL COMPONENTE JUMP
    // -------------------------------------------------
    
    /// <summary>
    /// Llamado cuando el componente Jump inicia un salto
    /// </summary>
    void OnJumpStarted()
    {
        state = State.Jumping;
        animator.SetBool("isJumping", true);
        animator.SetBool("isRushing", false);
    }
    
    /// <summary>
    /// Llamado cuando el componente Jump detecta que aterrizó
    /// </summary>
    void OnLanded()
    {
        animator.SetBool("isJumping", false);
        
        if (detector.JugadorDetectado)
        {
            if (detector.DistanciaAlJugador <= attackRange)
            {
                state = State.Attacking;
                mover.Move(0);
            }
            else if (detector.DistanciaAlJugador <= rushDetectionRange)
            {
                state = State.Rushing;
            }
            else
            {
                state = State.Chasing;
            }
        }
        else
        {
            state = State.Patrolling;
        }
    }
    
    // -------------------------------------------------
    
    public void DisableInput()
    {
        inputEnabled = false;
        mover.Move(0f); // se detiene al instante
        
        // Detener animaciones de movimiento
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isRushing", false);
            animator.SetBool("isJumping", false);
        }
        
        // Usar el componente Jump para detener el movimiento vertical
        if (jumpComponent != null)
        {
            jumpComponent.StopVerticalMovement();
        }
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }
    
    void OnDestroy()
    {
        // Desuscribirse de eventos para evitar memory leaks
        if (jumpComponent != null)
        {
            jumpComponent.OnJumpStarted -= OnJumpStarted;
            jumpComponent.OnLanded -= OnLanded;
        }
    }
    
    // -------------------------------------------------
    // MÉTODOS DE MUERTE Y CAÍDA
    // -------------------------------------------------
    
    /// <summary>
    /// Llama este método cuando el hongo muere (por ejemplo, desde Health o un Animation Event).
    /// Detiene toda la lógica de IA y permite que la animación de muerte se reproduzca.
    /// </summary>
    public void OnDeathFall()
    {
        // Desactivar la IA y el input inmediatamente
        DisableInput();
        
        // Detener todos los scripts de movimiento
        if (mover) mover.enabled = false;
        
        // Detener el componente Jump si existe
        if (jumpComponent) jumpComponent.enabled = false;
        
        // Detener cualquier movimiento del rigidbody pero permitir que caiga
        if (rb)
        {
            rb.linearVelocity = Vector2.zero; // Detener movimiento previo
            rb.gravityScale = 2.5f; // Permitir caída si está en el aire
        }
        
        // Asegurar que todas las animaciones de movimiento estén desactivadas
        if (animator)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isRushing", false);
            animator.SetBool("isJumping", false);
            // El trigger "death" ya debería haber sido activado por el componente Health
        }
        
        // Cambiar a estado de muerte para detener toda lógica de IA
        state = State.Dead;
        
        // Mensaje para efectos adicionales
        SendMessage("OnMushroomDeath", SendMessageOptions.DontRequireReceiver);
    }
}
