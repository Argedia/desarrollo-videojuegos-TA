using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1f;
    public float patrolRange = 5f;  // Rango de patrullaje (solo para enemigos que patrullan)
    public bool isPatrolling = true;  // Define si el enemigo patrulla o se mueve de alguna otra forma

    [Header("Components")]
    public Rigidbody2D rb;
    public FlipVisual flipVisual;

    private Vector2 moveDirection;

    public Animator animator; 

    void Awake()
    {
        // Asegurarse de que haya un FlipVisual en el enemigo
        flipVisual = GetComponent<FlipVisual>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Si el enemigo patrulla, le asignamos un movimiento inicial hacia la derecha o izquierda
        if (isPatrolling)
        {
            // Randomizar la dirección inicial del movimiento
            moveDirection = Random.Range(0f, 1f) > 0.5f ? Vector2.right : Vector2.left;
        }
    }

    void Update()
    {
        // Movimiento según dirección
        if (isPatrolling)
        {
            Patrol();
        }
        else
        {
            // Aquí puedes agregar lógica adicional para que el enemigo se mueva de manera diferente.
            // Por ejemplo, un movimiento hacia el jugador, si quieres un enemigo que persiga.
            // Por ahora dejamos el movimiento en línea recta.
            moveDirection = Vector2.right;  // Esto es solo un ejemplo
            Move();
        }

        // Flip visual según dirección del movimiento
        if (flipVisual != null)
        {
            flipVisual.FlipTo(moveDirection.x);
        }

        animator.SetBool("isMoving", Mathf.Abs(moveDirection.x) > 0.01f);
    }

    // Lógica de patrullaje, el enemigo se mueve de un lado a otro dentro de un rango.
    void Patrol()
    {
        float distance = Mathf.PingPong(Time.time * moveSpeed, patrolRange);
        moveDirection = (distance < patrolRange / 2) ? Vector2.left : Vector2.right;
        Move();
    }

    // Movimiento básico
    void Move()
    {
        // Actualizamos la velocidad en la dirección deseada
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
    }
}
