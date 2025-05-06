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

    private float contadorTiempo = 0.0f;
    private bool estaMoviendose = false;
    private float contadorMovimiento = 0.0f;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {

        // Mantén el resto del código original
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        contadorTiempo = 0;

        // Normaliza la dirección aquí si es necesario
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

                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = (direccion.x < 0);
                }
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

    public void SetDireccion(Vector2 nuevaDireccion)
    {
        direccion = nuevaDireccion.normalized;

        // Actualiza flip del sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direccion.x < 0);
        }
    }
}