using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public Button botonIniciar;
    public Button botonSalir;
    public Slider sliderVolumen;

    [Header("Configuración")]
    public string nombreEscenaJuego = "GameScene";

    // Referencia al AudioManager - esto es como tener un teléfono directo
    private AudioManager audioManager;

    void Start()
    {
        // Encontramos el AudioManager en la escena
        audioManager = FindObjectOfType<AudioManager>();

        // Si no hay AudioManager, creamos uno
        if (audioManager == null)
        {
            Debug.LogWarning("No se encontró AudioManager. Creando uno nuevo.");
            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManager = audioManagerObj.AddComponent<AudioManager>();
        }

        // Configuramos los botones con sus sonidos
        ConfigurarBotonesConAudio();

        // Iniciamos la música del menú
        audioManager.ReproducirMusicaMenu();

        // Configuramos el slider
        ConfigurarSliderVolumen();
    }

    void ConfigurarBotonesConAudio()
    {
        // Cada botón ahora tiene múltiples eventos: la acción principal y los sonidos

        // Botón Iniciar
        botonIniciar.onClick.AddListener(() => {
            audioManager.SonidoClickBoton();  // Primero el sonido
            audioManager.SonidoTransicion();  // Luego el sonido de transición
            IniciarJuego();  // Finalmente la acción
        });

        // Botón Salir
        botonSalir.onClick.AddListener(() => {
            audioManager.SonidoClickBoton();
            SalirDelJuego();
        });

        // Configuramos los sonidos de hover para ambos botones
        ConfigurarSonidosHover(botonIniciar);
        ConfigurarSonidosHover(botonSalir);
    }

    void ConfigurarSonidosHover(Button boton)
    {
        // Añadimos detectores de eventos para cuando el mouse entra y sale del botón
        UnityEngine.EventSystems.EventTrigger trigger = boton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        // Evento cuando el mouse entra
        UnityEngine.EventSystems.EventTrigger.Entry entryHover = new UnityEngine.EventSystems.EventTrigger.Entry();
        entryHover.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        entryHover.callback.AddListener((eventData) => { audioManager.SonidoHoverBoton(); });
        trigger.triggers.Add(entryHover);
    }

    void ConfigurarSliderVolumen()
    {
        // El slider ahora también reproduce sonidos cuando se ajusta
        sliderVolumen.onValueChanged.AddListener((valor) => {
            audioManager.CambiarVolumenMusica(valor);
            audioManager.SonidoSlider();  // Sonido cuando se mueve el slider
        });

        // Establecemos el valor inicial del slider
        sliderVolumen.value = PlayerPrefs.GetFloat("VolumenMusica", 0.7f);
    }

    void IniciarJuego()
    {
        // Cambiamos a la música del juego antes de cargar la escena
        audioManager.ReproducirMusicaJuego();

        // Pequeña pausa para que se escuche el efecto de transición
        StartCoroutine(CargarEscenaConDelay());
    }

    System.Collections.IEnumerator CargarEscenaConDelay()
    {
        // Esperamos un momento para que el sonido de transición se reproduzca
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    void SalirDelJuego()
    {
        // Guardamos todas las preferencias antes de salir
        PlayerPrefs.Save();
        Application.Quit();
    }
}