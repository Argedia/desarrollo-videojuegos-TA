using UnityEngine;

public class EnemigoMovimiento : MonoBehaviour
{

    [Header("Configuraci?n de Movimiento")]
    [Tooltip("Velocidad a la que se mueve el enemigo")]
    public float velocidad = 5.0f;

    [Tooltip("Direcci?n en la que se mover? el enemigo")]
    public Vector2 direccion = Vector2.right; // Por defecto se mueve hacia la derecha (1,0)

    [Header("Configuraci?n de Tiempo")]
    [Tooltip("Tiempo entre movimientos (en segundos)")]
    public float tiempoEntreMov = 2.0f;

    [Tooltip("Duraci?n de cada movimiento (en segundos)")]
    public float duracionMovimiento = 1.0f;

    [Header("Detección de Suelo")]
    public Transform puntoDeVisionSuelo;
    public float distanciaDeteccion = 1.0f;
    public LayerMask capaSuelo;

    private float contadorTiempo = 0.0f;
    private bool estaMoviendose = false;
    private float contadorMovimiento = 0.0f;
    private bool forzarCambioDireccion = false;  // New flag to force direction change
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {

        // Mant�n el resto del c�digo original
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        contadorTiempo = 0;

        // Normaliza la direcci�n aqu� si es necesario
        direccion = direccion.normalized;
    }

    void Update()
    {
        if (!estaMoviendose)
        {
            contadorTiempo += Time.deltaTime;

            if (contadorTiempo >= tiempoEntreMov)
            {
                contadorTiempo = 0;
                estaMoviendose = true;
                contadorMovimiento = 0;

                // Only change direction randomly if we're not forcing a direction change
                if (!forzarCambioDireccion && Random.value < 0.5f)
                {
                    direccion.x *= -1;
                    FlipSpriteAndCheckPoint();
                }

                forzarCambioDireccion = false; // Reset the flag
            }
        }
        else
        {
            contadorMovimiento += Time.deltaTime;

            MoverEnemigo();

            if (contadorMovimiento >= duracionMovimiento)
            {
                estaMoviendose = false;
                DetenerEnemigo();
            }
        }
    }
    private void MoverEnemigo()
    {
        if (!DetectGround())
        {
            DetenerEnemigo();
            direccion = -direccion; // Change direction
            FlipSpriteAndCheckPoint();
            estaMoviendose = false;
            contadorTiempo = 0f;
            forzarCambioDireccion = true; // Prevent random direction change in next movement
            return;
        }


        if (rb2d != null)
        {
            rb2d.linearVelocity = direccion * velocidad;
        }
        else
        {
            transform.Translate(direccion * velocidad * Time.deltaTime);
        }
    }

    private void DetenerEnemigo()
    {
        if (rb2d != null)
        {
            rb2d.linearVelocity = Vector2.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 dir3D = new Vector3(direccion.x, direccion.y, 0);
        Gizmos.DrawRay(transform.position, dir3D.normalized * 2);
    }

    private bool DetectGround()
    {
        Vector2 origen = puntoDeVisionSuelo != null ? puntoDeVisionSuelo.position : transform.position;
        Vector2 direccionRay = Vector2.down;

        // Check for ground at current position and ahead
        RaycastHit2D hitDown = Physics2D.Raycast(origen, direccionRay, distanciaDeteccion, capaSuelo);
        Vector2 origenAdelante = origen + (direccion * distanciaDeteccion);
        RaycastHit2D hitAhead = Physics2D.Raycast(origenAdelante, Vector2.down, distanciaDeteccion, capaSuelo);

        // Draw debug rays
        Debug.DrawRay(origen, direccionRay * distanciaDeteccion, Color.green);
        Debug.DrawRay(origenAdelante, Vector2.down * distanciaDeteccion, Color.yellow);

        // Return true only if there's ground below current position AND ahead
        return hitDown.collider != null && hitAhead.collider != null;
    }


    public void SetDireccion(Vector2 nuevaDireccion)
    {
        direccion = nuevaDireccion.normalized;

        // Actualiza flip del sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direccion.x < 0);
        }
    }

    private void FlipSpriteAndCheckPoint()
{
    if (spriteRenderer != null)
    {
        spriteRenderer.flipX = (direccion.x < 0);
    }

    if (puntoDeVisionSuelo != null)
    {
        Vector3 localPos = puntoDeVisionSuelo.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * (direccion.x < 0 ? -1 : 1);
        puntoDeVisionSuelo.localPosition = localPos;
    }
}

    
    
}