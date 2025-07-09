using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Referencias de UI - Arrastra aqu� desde el Inspector")]
    public GameObject pausePanel;              // El panel completo que contiene todo el men�
    public Button botonReanudar;              // Bot�n para continuar el juego
    public Button botonSalirMenu;             // Bot�n para volver al men� principal
    public Slider sliderVolumenPausa;         // Slider para ajustar volumen durante la pausa

    [Header("Configuraci�n de Escenas")]
    public string nombreEscenaMenu = "UI Testing";  // Nombre exacto de tu escena de men� principal

    // Referencias internas - no necesitas tocar estas en el Inspector
    private AudioManager audioManager;        // Referencia al AudioManager
    private bool isPaused = false;            // Estado actual del juego (pausado o no)

    void Start()
    {
        // Inicializaci�n - esto es como preparar todos los ingredientes antes de cocinar

        // Encontramos el AudioManager que ya existe en tu juego
        audioManager = FindObjectOfType<AudioManager>();

        // Verificamos que tengamos todo lo necesario
        if (audioManager == null)
        {
            Debug.LogError("�No se encontr� AudioManager! Aseg�rate de que est� en la escena.");
        }

        // Configuramos todos los botones con sus sonidos correspondientes
        ConfigurarBotonesConAudio();

        // Configuramos el slider de volumen para que funcione igual que en el men�
        ConfigurarSliderVolumen();

        // Importante: El panel de pausa debe estar oculto al iniciar
        pausePanel.SetActive(false);
    }

    void Update()
    {
        // Escuchamos constantemente si el jugador presiona Escape
        // Esta es la forma m�s com�n de pausar un juego
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // Esta funci�n es el coraz�n del sistema de pausa
        // Cambia entre pausado y no pausado

        isPaused = !isPaused;  // Invertimos el estado actual

        // Mostramos u ocultamos el panel seg�n el estado
        pausePanel.SetActive(isPaused);

        // CR�TICO: Time.timeScale controla la velocidad del tiempo en Unity
        // 0 = tiempo completamente pausado
        // 1 = tiempo normal
        Time.timeScale = isPaused ? 0f : 1f;

        // Reproducimos sonido de confirmaci�n cuando pausamos
        if (isPaused && audioManager != null)
        {
            audioManager.SonidoClickBoton();
        }
    }

    void ConfigurarBotonesConAudio()
    {
        // Configuramos cada bot�n con su funcionalidad y sonidos
        // Esto es similar a como lo hiciste en MainMenuManager

        // Bot�n Reanudar - simplemente quita la pausa
        botonReanudar.onClick.AddListener(() => {
            if (audioManager != null)
            {
                audioManager.SonidoClickBoton();  // Sonido de confirmaci�n
            }
            TogglePause();  // Salimos del estado de pausa
        });

        // Bot�n Salir al Men� - m�s complejo porque cambia de escena
        botonSalirMenu.onClick.AddListener(() => {
            if (audioManager != null)
            {
                audioManager.SonidoClickBoton();        // Sonido de clic
                audioManager.SonidoTransicion();        // Sonido de transici�n
                audioManager.ReproducirMusicaMenu();    // Cambiamos a m�sica de men�
            }

            // IMPORTANTE: Restauramos el tiempo antes de cambiar escena
            // Si no hacemos esto, el men� tambi�n estar�a pausado
            Time.timeScale = 1f;

            // Cambiamos a la escena del men� principal
            SceneManager.LoadScene(nombreEscenaMenu);
        });

        // A�adimos sonidos de hover para mejor experiencia de usuario
        ConfigurarSonidosHover(botonReanudar);
        ConfigurarSonidosHover(botonSalirMenu);
    }

    void ConfigurarSonidosHover(Button boton)
    {
        // Esta funci�n a�ade el sonido cuando el mouse pasa sobre el bot�n
        // Es exactamente igual que en tu MainMenuManager

        UnityEngine.EventSystems.EventTrigger trigger = boton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        // Creamos el evento para cuando el mouse entra al bot�n
        UnityEngine.EventSystems.EventTrigger.Entry entryHover = new UnityEngine.EventSystems.EventTrigger.Entry();
        entryHover.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        entryHover.callback.AddListener((eventData) => {
            if (audioManager != null)
            {
                audioManager.SonidoHoverBoton();
            }
        });

        trigger.triggers.Add(entryHover);
    }

    void ConfigurarSliderVolumen()
    {
        // El slider de volumen funciona exactamente igual que en el men� principal
        // Reutilizamos las mismas funciones del AudioManager

        sliderVolumenPausa.onValueChanged.AddListener((valor) => {
            if (audioManager != null)
            {
                audioManager.CambiarVolumenMusica(valor);  // Cambiamos el volumen
                audioManager.SonidoSlider();               // Sonido de feedback
            }
        });

        // Establecemos el valor inicial del slider basado en las preferencias guardadas
        sliderVolumenPausa.value = PlayerPrefs.GetFloat("VolumenMusica", 0.7f);
    }

    // Funci�n p�blica para que otros scripts puedan pausar el juego
    public void PausarJuego()
    {
        if (!isPaused)
        {
            TogglePause();
        }
    }

    // Funci�n p�blica para que otros scripts puedan reanudar el juego
    public void ReanudarJuego()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    // Propiedad para que otros scripts puedan verificar si el juego est� pausado
    public bool EstaEnPausa
    {
        get { return isPaused; }
    }
}