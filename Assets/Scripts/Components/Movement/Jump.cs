using UnityEngine;

/// <summary>
/// Componente para gestionar saltos de entidades (jugador, enemigos, etc.)
/// Proporciona una interfaz consistente para ejecutar saltos con diferentes parámetros
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Jump : MonoBehaviour
{
    [Header("Configuración de Salto")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private bool useGroundDetection = true;
    
    [Header("Detección de Suelo (Opcional)")]
    [SerializeField] private LayerMask groundLayerMask = 1; // Default layer
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 groundCheckOffset = Vector2.zero;
    
    // Referencias
    private Rigidbody2D rb;
    private Animator animator;
    
    // Estado interno
    private float lastJumpTime = -10f;
    private bool isGrounded = true;
    
    // Propiedades públicas
    public bool IsGrounded => isGrounded;
    public bool CanJump => (!useGroundDetection || isGrounded) && Time.time - lastJumpTime >= jumpCooldown;
    public bool IsJumping => rb.linearVelocity.y > 0.1f;
    
    // Eventos
    public System.Action OnJumpStarted;
    public System.Action OnLanded;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (useGroundDetection)
        {
            CheckGrounded();
        }
    }
    
    /// <summary>
    /// Ejecuta un salto vertical simple
    /// </summary>
    public bool TryJump()
    {
        return TryJump(Vector2.up * jumpForce);
    }
    
    /// <summary>
    /// Ejecuta un salto con fuerza personalizada
    /// </summary>
    public bool TryJump(float customJumpForce)
    {
        return TryJump(Vector2.up * customJumpForce);
    }
    
    /// <summary>
    /// Ejecuta un salto en una dirección específica
    /// </summary>
    public bool TryJump(Vector2 jumpDirection)
    {
        if (!CanJump) return false;
        
        // Aplicar fuerza de salto
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + jumpDirection.x, jumpDirection.y);
        
        // Actualizar estado
        lastJumpTime = Time.time;
        isGrounded = false;
        
        // Activar animación si existe
        if (animator != null)
        {
            animator.SetBool("isJumping", true);
            animator.SetTrigger("Jump");
        }
        
        // Disparar evento
        OnJumpStarted?.Invoke();
        
        return true;
    }
    
    /// <summary>
    /// Ejecuta un salto hacia una posición específica
    /// </summary>
    public bool TryJumpToPosition(Vector2 targetPosition, float forceMultiplier = 1f)
    {
        if (!CanJump) return false;
        
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        
        // Asegurar componente vertical mínima
        if (direction.y < 0.3f)
            direction.y = 0.6f;
            
        Vector2 jumpForceVector = direction * jumpForce * forceMultiplier;
        
        return TryJump(jumpForceVector);
    }
    
    /// <summary>
    /// Fuerza al componente a considerar que está en el suelo
    /// </summary>
    public void ForceGrounded(bool grounded = true)
    {
        isGrounded = grounded;
        
        if (grounded && animator != null)
        {
            animator.SetBool("isJumping", false);
            OnLanded?.Invoke();
        }
    }
    
    /// <summary>
    /// Verifica si está tocando el suelo usando raycast
    /// </summary>
    private void CheckGrounded()
    {
        Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayerMask);
        
        bool wasGrounded = isGrounded;
        isGrounded = hit.collider != null || rb.linearVelocity.y <= 0.1f;
        
        // Si acabamos de aterrizar
        if (!wasGrounded && isGrounded)
        {
            if (animator != null)
            {
                animator.SetBool("isJumping", false);
            }
            OnLanded?.Invoke();
        }
    }
    
    /// <summary>
    /// Configuración personalizada de parámetros de salto
    /// </summary>
    public void ConfigureJump(float newJumpForce, float newCooldown)
    {
        jumpForce = newJumpForce;
        jumpCooldown = newCooldown;
    }
    
    /// <summary>
    /// Detiene cualquier movimiento vertical (útil para cancelar saltos)
    /// </summary>
    public void StopVerticalMovement()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }
    
    // Gizmos para debugging
    void OnDrawGizmosSelected()
    {
        if (useGroundDetection)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * groundCheckDistance);
            Gizmos.DrawWireSphere(rayOrigin + Vector2.down * groundCheckDistance, 0.1f);
        }
    }
}
