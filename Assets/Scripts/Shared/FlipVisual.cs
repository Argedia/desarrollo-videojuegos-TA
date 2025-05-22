using UnityEngine;

public class FlipVisual : MonoBehaviour
{
    [SerializeField] private Transform visualTransform;
    private bool facingRight = true;

    private void Awake()
    {
        // Intentar obtener automáticamente el Transform del objeto hijo "Visual" (si existe)
        visualTransform = transform.Find("Visual");

        // Si no se encuentra "Visual", buscará el propio transform del objeto actual
        if (visualTransform == null){
            Debug.Log("No se econtró hijo llamado Visual");
            visualTransform = transform;
        }
    }

    public void FlipTo(float direction)
    {
        if (direction > 0 && !facingRight)
            Flip();
        else if (direction < 0 && facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = visualTransform.localScale;
        scale.x *= -1;
        visualTransform.localScale = scale;
    }

    public bool IsFacingRight() => facingRight;
}