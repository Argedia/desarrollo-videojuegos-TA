using UnityEngine;

public class SimpleLevelBackground : MonoBehaviour
{
    [Header("Configuración")]
    public Transform puntoInicialCamara;
    public float velocidadMovimiento = 1f;
    public float rangoMovimiento = 10f;
    public float tiempoEntrePuntos = 4f;

    private Camera cam;
    private Vector3 puntoObjetivo;
    private float timer;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Configurar cámara para fondo
        cam.depth = -10;
        cam.cullingMask = -1; // Ver todo

        // Posición inicial
        if (puntoInicialCamara != null)
        {
            transform.position = puntoInicialCamara.position;
            transform.rotation = puntoInicialCamara.rotation;
        }

        GenerarNuevoPunto();
    }

    void Update()
    {
        // Mover hacia punto objetivo
        transform.position = Vector3.Lerp(transform.position, puntoObjetivo, velocidadMovimiento * Time.deltaTime);

        // Cambiar punto
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            GenerarNuevoPunto();
            timer = tiempoEntrePuntos;
        }
    }

    void GenerarNuevoPunto()
    {
        Vector3 posicionBase = puntoInicialCamara != null ? puntoInicialCamara.position : transform.position;

        puntoObjetivo = posicionBase + new Vector3(
            Random.Range(-rangoMovimiento, rangoMovimiento),
            Random.Range(-rangoMovimiento / 2, rangoMovimiento / 2),
            Random.Range(-rangoMovimiento, rangoMovimiento)
        );

        timer = tiempoEntrePuntos;
    }
}