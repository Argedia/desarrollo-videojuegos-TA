using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public Button botonIniciar;
    public Button botonSalir;
    public Slider sliderVolumen;

    [Header("Configuraci�n")]
    public string nombreEscenaJuego = "GameScene";

    // Referencia al AudioManager - esto es como tener un tel�fono directo
    private AudioManager audioManager;

    void Start()
    {
        // Encontramos el AudioManager en la escena
        audioManager = FindObjectOfType<AudioManager>();

        // Si no hay AudioManager, creamos uno
        if (audioManager == null)
        {
            Debug.LogWarning("No se encontr� AudioManager. Creando uno nuevo.");
            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManager = audioManagerObj.AddComponent<AudioManager>();
        }

        // Configuramos los botones con sus sonidos
        ConfigurarBotonesConAudio();

        // Iniciamos la m�sica del men�
        audioManager.ReproducirMusicaMenu();

        // Configuramos el slider
        ConfigurarSliderVolumen();
    }

    void ConfigurarBotonesConAudio()
    {
        // Cada bot�n ahora tiene m�ltiples eventos: la acci�n principal y los sonidos

        // Bot�n Iniciar
        botonIniciar.onClick.AddListener(() => {
            audioManager.SonidoClickBoton();  // Primero el sonido
            audioManager.SonidoTransicion();  // Luego el sonido de transici�n
            IniciarJuego();  // Finalmente la acci�n
        });

        // Bot�n Salir
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
        // A�adimos detectores de eventos para cuando el mouse entra y sale del bot�n
        UnityEngine.EventSystems.EventTrigger trigger = boton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        // Evento cuando el mouse entra
        UnityEngine.EventSystems.EventTrigger.Entry entryHover = new UnityEngine.EventSystems.EventTrigger.Entry();
        entryHover.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        entryHover.callback.AddListener((eventData) => { audioManager.SonidoHoverBoton(); });
        trigger.triggers.Add(entryHover);
    }

    void ConfigurarSliderVolumen()
    {
        // El slider ahora tambi�n reproduce sonidos cuando se ajusta
        sliderVolumen.onValueChanged.AddListener((valor) => {
            audioManager.CambiarVolumenMusica(valor);
            audioManager.SonidoSlider();  // Sonido cuando se mueve el slider
        });

        // Establecemos el valor inicial del slider
        sliderVolumen.value = PlayerPrefs.GetFloat("VolumenMusica", 0.7f);
    }

    void IniciarJuego()
    {
        // Cambiamos a la m�sica del juego antes de cargar la escena
        audioManager.ReproducirMusicaJuego();

        // Peque�a pausa para que se escuche el efecto de transici�n
        StartCoroutine(CargarEscenaConDelay());
    }

    System.Collections.IEnumerator CargarEscenaConDelay()
    {
        // Esperamos un momento para que el sonido de transici�n se reproduzca
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