using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public FlipVisual flipVisual;  // Referencia al script FlipVisual

    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isGrounded = false;
    public MeleeAttack meleeAttack;
    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump();

        inputActions.Player.Attack.performed += ctx => meleeAttack?.TryAttack();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        
        // Intentar obtener automáticamente el script FlipVisual
        flipVisual = GetComponent<FlipVisual>();
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        // Movimiento horizontal
        Vector2 velocity = rb.linearVelocity; // Cambié a rb.velocity (en vez de linearVelocity)
        velocity.x = moveInput.x * moveSpeed;
        rb.linearVelocity = velocity;  // Aplico la nueva velocidad

        // Flip visual del sprite según dirección de movimiento
        if (flipVisual != null)
        {
            flipVisual.FlipTo(moveInput.x);
        }

        // Parámetros de animación
        if (animator != null)
        {
            animator.SetBool("isMoving", moveInput.x != 0);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("verticalVelocity", rb.linearVelocity.y);  // Cambié a rb.velocity
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);  // Cambié a rb.velocity
            isGrounded = false;
        }
    }

    // Detecta la entrada en la colisión con el suelo
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobamos la normal de la colisión
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Solo marca como grounded si la colisión es desde abajo
            if (collision.contacts[0].normal.y > 0.5f) // Asegura que la normal sea hacia arriba
            {
                isGrounded = true;
            }
        }
    }
}
