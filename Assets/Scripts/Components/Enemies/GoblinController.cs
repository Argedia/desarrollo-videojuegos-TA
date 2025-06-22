using UnityEngine;

/// <summary>
/// Inteligencia básica para un goblin 2D:
///  • Patrulla en línea recta sobre la plataforma.
///  • Si detecta al jugador, lo persigue sin suicidarse en un borde.
///  • Cuando está a rango, entra en modo “Ataque” (solo lanza un evento; la animación/golpe vive en otro script).
/// </summary>
[RequireComponent(typeof(UniformHorizontalMovement))]
[RequireComponent(typeof(EdgeDetector))]
[RequireComponent(typeof(EnemyPlayerDetector))]
public class GoblinController : Controller, IController
{
    private bool inputEnabled = true;
    // --- Parámetros de balance ---
    [Header("Velocidades")]
    [SerializeField] float patrolSpeed = 1.5f;
    [SerializeField] float chaseSpeed = 3f;

    [Header("Rangos")]
    [SerializeField] float attackRange = 1.2f;

    // --- Referencias a los componentes auxiliares ---
    UniformHorizontalMovement mover;
    EdgeDetector edge;
    EnemyPlayerDetector detector;

    // --- Estado interno ---
    enum State { Patrolling, Chasing, Attacking }
    State state = State.Patrolling;


    // -------------------------------------------------
    void Awake()
    {
        mover = GetComponent<UniformHorizontalMovement>();
        edge = GetComponent<EdgeDetector>();
        detector = GetComponent<EnemyPlayerDetector>();
    }

    void Start()
    {
        // Garantiza que el movimiento arranque en la dirección inicial.
        mover.Move(facingDir);
    }

    void Update()
    {
        if (!inputEnabled) return;

        switch (state)
        {
            case State.Patrolling:
                DoPatrol();
                break;
            case State.Chasing:
                DoChase();
                break;
            case State.Attacking:
                DoAttack();
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

        //¿Vio al jugador?
        if (detector.JugadorDetectado)
            state = State.Chasing;
    }

    void DoChase()
    {
        // Elegimos la dirección hacia el jugador
        int dirToPlayer = detector.player.position.x > transform.position.x ? 1 : -1;

        // Si no hay suelo delante y seguiríamos cayendo, nos detenemos
        if (!edge.isGroundAhead && dirToPlayer == facingDir)
        {
            mover.Move(0);   // se frena en el borde
            return;
        }

        // Si la dirección cambió con respecto al “facingDir”, flip visual
        if (dirToPlayer != facingDir)
            Flip();

        mover.Move(facingDir);

        // ¿Está suficientemente cerca para atacar?
        if (detector.DistanciaAlJugador <= attackRange)
        {
            mover.Move(0);
            state = State.Attacking;
        }
        // ¿Perdió al jugador?
        else if (!detector.JugadorDetectado)
        {
            state = State.Patrolling;
            mover.Move(facingDir);
        }
    }

    void DoAttack()
    {
        // Aquí no nos movemos: golpea otro script que escuche este evento.
        // Ejemplo: BroadcastMessage, UnityEvent, etc.
        SendMessage("OnGoblinAttack", SendMessageOptions.DontRequireReceiver);

        // Si el jugador se aleja, volvemos a persecución o patrulla
        if (detector.DistanciaAlJugador > attackRange * 1.3f)
            state = detector.JugadorDetectado ? State.Chasing : State.Patrolling;
    }

    // -------------------------------------------------
    
    
    public void DisableInput()
    {
        inputEnabled = false;
        mover.Move(0f); // se detiene al instante
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }
}
