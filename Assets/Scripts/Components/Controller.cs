using UnityEngine;

public class Controller : MonoBehaviour
{
    //  +1 si mira a la derecha, -1 a la izquierda.
    protected int facingDir = 1;
    protected void Flip()
    {
        facingDir *= -1;

        // Si tu sprite usa escala local para flip visual
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDir;
        transform.localScale = scale;
    }
}
