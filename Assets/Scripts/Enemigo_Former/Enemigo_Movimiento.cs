using UnityEngine;

public class EnemigoMovimiento : PlayerMovement
{
    [Header("Configuración de Tiempo")]
    public float tiempoEntreMov = 2.0f;
    public float duracionMovimiento = 1.0f;

    [Header("Detección de Suelo")]
    public Transform puntoDeVisionSuelo;
    public float distanciaDeteccion = 1.0f;

    private Vector2 direccion;
    private float contadorTiempo = 0f;
    private float contadorMovimiento = 0f;
    private bool forzarCambioDireccion = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        direccion = Random.value < 0.5f ? Vector2.left : Vector2.right;
        isMovementPaused = true; // Inicia en pausa para esperar el primer movimiento
    }

    private void Update()
    {
        if (isMovementPaused)
        {
            contadorTiempo += Time.deltaTime;
            if (contadorTiempo >= tiempoEntreMov)
            {
                contadorTiempo = 0f;
                isMovementPaused = false;
                contadorMovimiento = 0f;

                if (!forzarCambioDireccion && Random.value < 0.5f)
                {
                    FlipCheckPoint();
                }

                forzarCambioDireccion = false;
            }

            Run(0f);
        }
        else
        {
            contadorMovimiento += Time.deltaTime;

            if (!DetectGround())
            {
                FlipCheckPoint();

                isMovementPaused = true;
                contadorTiempo = 0f;
                forzarCambioDireccion = true;
                Run(0f);
                return;
            }

            Run(direccion.x);

            if (contadorMovimiento >= duracionMovimiento)
            {
                isMovementPaused = true;
                Run(0f);
            }
        }
    }

    private bool DetectGround()
    {
        Vector2 origen = puntoDeVisionSuelo != null ? puntoDeVisionSuelo.position : transform.position;
        Vector2 direccionRay = Vector2.down;

        RaycastHit2D hitDown = Physics2D.Raycast(origen, direccionRay, distanciaDeteccion, groundLayer);
        Vector2 origenAdelante = origen + (direccion * distanciaDeteccion);
        RaycastHit2D hitAhead = Physics2D.Raycast(origenAdelante, Vector2.down, distanciaDeteccion, groundLayer);

        Debug.DrawRay(origen, direccionRay * distanciaDeteccion, Color.green);
        Debug.DrawRay(origenAdelante, Vector2.down * distanciaDeteccion, Color.yellow);

        return hitDown.collider != null && hitAhead.collider != null;
    }

    private void FlipCheckPoint()
    {
        direccion.x *= -1;
        if (puntoDeVisionSuelo != null)
        {
            Vector3 localPos = puntoDeVisionSuelo.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * (direccion.x < 0 ? -1 : 1);
            puntoDeVisionSuelo.localPosition = localPos;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direccion.x < 0);
        }
    }
}
