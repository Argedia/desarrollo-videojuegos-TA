using System;
using UnityEngine;

public class Player_Mov_Animation : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isGrounded = false;
    private bool jumpRequested = false;
    private float horizontalInput = 0f; // Esto para modificarlo luego en el FixedUpdate


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }
        Flip();
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isJumping", !isGrounded);
    }

    // Todo lo que tega que ver con interaccion de entorno y fisicas, van aca.
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            jumpRequested = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void Flip()
    {
        if(rb.linearVelocityX > 0.5f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.linearVelocityX < 0.5f)
        { 
            spriteRenderer.flipX = true;
        }
    }
}
