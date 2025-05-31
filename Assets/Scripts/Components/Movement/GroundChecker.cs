using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayLength = 0.2f;
    [SerializeField] private Vector2 rayOriginOffset = Vector2.zero;

    public bool IsGrounded { get; private set; }

    private void Update()
    {
        Vector2 origin = (Vector2)transform.position + rayOriginOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);

        IsGrounded = hit.collider != null;

        // Opcional: dibujar el raycast en la escena para debug
        Debug.DrawRay(origin, Vector2.down * rayLength, IsGrounded ? Color.green : Color.red);
    }
}
