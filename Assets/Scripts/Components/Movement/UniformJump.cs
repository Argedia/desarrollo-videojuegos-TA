using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UniformJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private GroundChecker groundChecker;
    Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (groundChecker == null)
        {
            groundChecker = GetComponent<GroundChecker>();
        }

        if (groundChecker == null)
        {
            Debug.LogWarning($"[Jump] No GroundChecker found on {gameObject.name}. Jumping won't work.");
        }

        anim = GetComponent<Animator>();
    }

    public void TryJump()
    {
        if (groundChecker != null && groundChecker.IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f); // Reset vertical velocity
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void Update()
    {
        anim.SetFloat("yVelocity", rb.linearVelocityY);   
    }
}
