using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private Vector2 boxOffset = new Vector2(0f, -0.5f);
    Animator anim;
    public bool IsGrounded { get; private set; }
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Vector2 origin = (Vector2)transform.position + boxOffset;
        IsGrounded = Physics2D.OverlapBox(origin, boxSize, 0f, groundLayer);
        anim?.SetBool("isGrounded", IsGrounded);   
        Debug.DrawLine(origin + new Vector2(-boxSize.x / 2, 0), origin + new Vector2(boxSize.x / 2, 0), IsGrounded ? Color.green : Color.red);
        Debug.DrawLine(origin + new Vector2(-boxSize.x / 2, -boxSize.y), origin + new Vector2(boxSize.x / 2, -boxSize.y), IsGrounded ? Color.green : Color.red);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + boxOffset, boxSize);
    }
}
