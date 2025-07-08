using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("M�sica de Fondo")]
    public AudioClip musicaMenu;
    public AudioClip musicaJuego;

    [Header("Efectos de Sonido de UI")]
    public AudioClip sonidoClickBoton;
    public AudioClip sonidoHoverBoton;
    public AudioClip sonidoSlider;
    public AudioClip sonidoTransicion;

    [Header("Configuraci�n de Audio")]
    public float volumenMusicaDefault = 0.7f;
    public float volumenEfectosDefault = 0.8f;
    public float tiempoFadeMusica = 0.5f;

    // Componentes internos - estas son las "herramientas" que usaremos
    private AudioSource fuenteMusica;
    private AudioSource fuenteEfectos;
    private AudioSource fuenteUI;

    // Variables para controlar el estado del audio
    private bool musicaActivada = true;
    private bool efectosActivados = true;

    void Awake()
    {
        // Awake se ejecuta antes que Start, perfecto para inicializaci�n
        // Nos aseguramos de que solo haya un AudioManager en toda la escena
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Hacemos que este objeto persista entre escenas
        // Esto significa que la m�sica continuar� cuando cambies de men� a juego
        DontDestroyOnLoad(gameObject);

        // Configuramos nuestras fuentes de audio
        ConfigurarFuentesAudio();

        // Cargamos las preferencias de audio del jugador
        CargarPreferenciasAudio();
    }

    void ConfigurarFuentesAudio()
    {
        // Creamos tres AudioSources separados para diferentes tipos de audio
        // Esto nos da control granular sobre cada categor�a

        // Fuente para m�sica de fondo
        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteMusica.loop = true;  // La m�sica se repite infinitamente
        fuenteMusica.volume = volumenMusicaDefault;
        fuenteMusica.priority = 64;  // Prioridad media

        // Fuente para efectos de sonido generales
        fuenteEfectos = gameObject.AddComponent<AudioSource>();
        fuenteEfectos.loop = false;  // Los efectos se reproducen una vez
        fuenteEfectos.volume = volumenEfectosDefault;
        fuenteEfectos.priority = 128;  // Prioridad alta

        // Fuente espec�fica para sonidos de UI
        fuenteUI = gameObject.AddComponent<AudioSource>();
        fuenteUI.loop = false;
        fuenteUI.volume = volumenEfectosDefault;
        fuenteUI.priority = 200;  // Prioridad muy alta - los sonidos de UI son cr�ticos
    }

    void CargarPreferenciasAudio()
    {
        // Recuperamos las preferencias que el jugador hab�a configurado
        musicaActivada = PlayerPrefs.GetInt("MusicaActivada", 1) == 1;
        efectosActivados = PlayerPrefs.GetInt("EfectosActivados", 1) == 1;

        float volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", volumenMusicaDefault);
        float volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", volumenEfectosDefault);

        // Aplicamos estas preferencias a nuestras fuentes
        fuenteMusica.volume = musicaActivada ? volumenMusica : 0f;
        fuenteEfectos.volume = efectosActivados ? volumenEfectos : 0f;
        fuenteUI.volume = efectosActivados ? volumenEfectos : 0f;
    }

    public void ReproducirMusicaMenu()
    {
        // Esta funci�n inicia la m�sica del men� con una transici�n suave
        if (musicaMenu != null && musicaActivada)
        {
            StartCoroutine(CambiarMusicaConFade(musicaMenu));
        }
    }

    public void ReproducirMusicaJuego()
    {
        // Cambia a la m�sica del juego cuando el jugador inicia una partida
        if (musicaJuego != null && musicaActivada)
        {
            StartCoroutine(CambiarMusicaConFade(musicaJuego));
        }
    }

    IEnumerator CambiarMusicaConFade(AudioClip nuevaMusica)
    {
        // Este es un efecto profesional: la m�sica actual se desvanece gradualmente
        // mientras la nueva m�sica aparece gradualmente

        // Fase 1: Desvanecemos la m�sica actual
        float volumenOriginal = fuenteMusica.volume;
        while (fuenteMusica.volume > 0.01f)
        {
            fuenteMusica.volume -= volumenOriginal * Time.deltaTime / tiempoFadeMusica;
            yield return null;  // Esperamos un frame
        }

        // Fase 2: Cambiamos la m�sica
        fuenteMusica.clip = nuevaMusica;
        fuenteMusica.Play();

        // Fase 3: Hacemos aparecer gradualmente la nueva m�sica
        while (fuenteMusica.volume < volumenOriginal)
        {
            fuenteMusica.volume += volumenOriginal * Time.deltaTime / tiempoFadeMusica;
            yield return null;
        }

        // Nos aseguramos de que el volumen sea exactamente el correcto
        fuenteMusica.volume = volumenOriginal;
    }

    public void SonidoClickBoton()
    {
        // Reproduce el sonido cuando el jugador hace clic en un bot�n
        if (sonidoClickBoton != null && efectosActivados)
        {
            fuenteUI.PlayOneShot(sonidoClickBoton);
        }
    }

    public void SonidoHoverBoton()
    {
        // Sonido sutil cuando el mouse pasa sobre un bot�n
        if (sonidoHoverBoton != null && efectosActivados)
        {
            fuenteUI.PlayOneShot(sonidoHoverBoton);
        }
    }

    public void SonidoSlider()
    {
        // Sonido cuando el jugador ajusta el slider de volumen
        if (sonidoSlider != null && efectosActivados)
        {
            fuenteUI.PlayOneShot(sonidoSlider);
        }
    }

    public void SonidoTransicion()
    {
        // Sonido especial para transiciones entre escenas
        if (sonidoTransicion != null && efectosActivados)
        {
            fuenteEfectos.PlayOneShot(sonidoTransicion);
        }
    }

    public void CambiarVolumenMusica(float nuevoVolumen)
    {
        // Esta funci�n ser� llamada por tu slider de volumen
        if (musicaActivada)
        {
            fuenteMusica.volume = nuevoVolumen;
        }

        // Guardamos la preferencia
        PlayerPrefs.SetFloat("VolumenMusica", nuevoVolumen);
    }

    public void CambiarVolumenEfectos(float nuevoVolumen)
    {
        // Controla el volumen de todos los efectos de sonido
        if (efectosActivados)
        {
            fuenteEfectos.volume = nuevoVolumen;
            fuenteUI.volume = nuevoVolumen;
        }

        PlayerPrefs.SetFloat("VolumenEfectos", nuevoVolumen);
    }

    public void ToggleMusica()
    {
        // Permite al jugador activar/desactivar completamente la m�sica
        musicaActivada = !musicaActivada;

        if (musicaActivada)
        {
            fuenteMusica.volume = PlayerPrefs.GetFloat("VolumenMusica", volumenMusicaDefault);
        }
        else
        {
            fuenteMusica.volume = 0f;
        }

        PlayerPrefs.SetInt("MusicaActivada", musicaActivada ? 1 : 0);
    }

    public void ToggleEfectos()
    {
        // Permite al jugador activar/desactivar los efectos de sonido
        efectosActivados = !efectosActivados;

        float volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", volumenEfectosDefault);

        if (efectosActivados)
        {
            fuenteEfectos.volume = volumenEfectos;
            fuenteUI.volume = volumenEfectos;
        }
        else
        {
            fuenteEfectos.volume = 0f;
            fuenteUI.volume = 0f;
        }

        PlayerPrefs.SetInt("EfectosActivados", efectosActivados ? 1 : 0);
    }
}