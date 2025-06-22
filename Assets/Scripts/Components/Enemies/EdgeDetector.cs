using UnityEngine;

public class EdgeDetector : MonoBehaviour
{
    [Header("Detección de borde")]
    public Vector2 rayOriginOffset = new Vector2(0.5f, -0.5f); // Offset local respecto al transform
    public float checkDistance = 0.1f;
    public LayerMask groundLayer;

    [Header("Estado")]
    public bool isGroundAhead = true;

    void FixedUpdate()
    {
        isGroundAhead = CheckGroundAhead();
    }

    bool CheckGroundAhead()
    {
        Vector2 origin = transform.TransformPoint(rayOriginOffset); // Respeta la rotación y escala local
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, checkDistance, groundLayer);
        return hit.collider != null;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 origin = transform.TransformPoint(rayOriginOffset);
        Gizmos.color = isGroundAhead ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + Vector2.down * checkDistance);
    }
}
