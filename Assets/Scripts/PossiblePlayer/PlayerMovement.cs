using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 15f;
    public float maxJumpHoldTime = 0.25f;
    public float minJumpForce = 5f; // fuerza mínima para salto corto

    [Header("Ground Check")]
    public LayerMask groundLayer;

    protected Rigidbody2D rb;
    private bool isGrounded = false;

    private float horizontalInput = 0f;
    protected bool isMovementPaused = false;

    // Variables para salto variable
    private bool isJumping = false;
    private float jumpHoldTimer = 0f;
    private bool jumpPressedLastFrame = false;
    //Variables para knockback
    private bool isKnockbackActive = false;
    private Vector2 knockbackVelocity = Vector2.zero;

    private PlayerAnimatorController animController;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponentInChildren<PlayerAnimatorController>();
    }

    private void FixedUpdate()
    {
        if (isKnockbackActive)
        {
            rb.linearVelocity = knockbackVelocity;
            return;
        }
        HandleRunning();
        HandleJump();

        // Señales al animador
        animController?.SetIsGrounded(isGrounded);
        animController?.SetVerticalVelocity(rb.linearVelocityY);
        animController?.SetIsMoving(Mathf.Abs(horizontalInput) > 0.1f);
    }

    private void HandleRunning()
    {
        if (isMovementPaused)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocityY);

    }

    private void HandleJump()
    {
        if (isJumping)
        {
            jumpHoldTimer += Time.fixedDeltaTime;

            if (jumpHoldTimer < maxJumpHoldTime)
            {
                // Aplica fuerza extra mientras el botón siga presionado y no se pase maxHoldTime
                rb.AddForce(Vector2.up * jumpForce * Time.fixedDeltaTime, ForceMode2D.Force);
            }
            else
            {
                // Tiempo máximo alcanzado → termina salto
                isJumping = false;
            }
        }
    }

    public void Run(float horizontal)
    {
        if (isMovementPaused)
            return;

        horizontalInput = horizontal;
        // Animación de orientación
        animController?.SetFacing(horizontalInput);
    }

    public void Jump(bool jumpPressed)
    {
        if (isMovementPaused) return;
        if (jumpPressed)
        {
            if (!jumpPressedLastFrame && isGrounded)
            {
                // Inicio de salto
                isJumping = true;
                jumpHoldTimer = 0f;
                isGrounded = false;

                // Aplicar impulso inicial
                rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f); // reset velocidad Y
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            if (isJumping)
            {
                // El jugador soltó el botón antes de maxJumpHoldTime → salto corto
                if (jumpHoldTimer < 0.1f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
                    rb.AddForce(Vector2.up * minJumpForce, ForceMode2D.Impulse);
                }
                isJumping = false;
            }
        }

        jumpPressedLastFrame = jumpPressed;
    }

    // Ground check solo con colisión desde arriba
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    // Pausar movimiento: duración en segundos, si <=0 pausa indefinidamente
    public void Freeze(float duration = 0f)
    {
        isMovementPaused = true;
        horizontalInput = 0f;

        if (duration > 0f)
        {
            StopAllCoroutines();
            StartCoroutine(ResumeAfterDelay(duration));
        }
    }

    private IEnumerator ResumeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FreeMove();
    }

    // Reanudar movimiento manualmente
    public void FreeMove()
    {
        isMovementPaused = false;
    }
    
    public void ApplyKnockback(Vector2 force, float duration = 0.2f)
    {
        knockbackVelocity = force;
        isKnockbackActive = true;
        Freeze(0.5f);
        StartCoroutine(EndKnockbackAfterDelay(duration));
    }

    private IEnumerator EndKnockbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isKnockbackActive = false;
        knockbackVelocity = Vector2.zero;
    }
}
