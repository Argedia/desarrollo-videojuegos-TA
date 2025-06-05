using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UniformHorizontalMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float currentDirection = 0f;

    /// <summary>
    /// Indica si actualmente se está moviendo horizontalmente.
    /// </summary>
    public bool IsMoving => Mathf.Abs(currentDirection) > 0.01f;
    private Animator animator;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Define la dirección horizontal del movimiento. -1: izquierda, 0: detenido, 1: derecha.
    /// </summary>
    public void Move(float direction)
    {
        currentDirection = Mathf.Clamp(direction, -1f, 1f);
            animator.SetBool("isMoving", direction!=0);
        
        
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = currentDirection * moveSpeed;
        rb.linearVelocity = velocity;
    }
}
