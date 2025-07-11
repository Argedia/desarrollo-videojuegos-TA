using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Referencias de UI - Arrastra aquí desde el Inspector")]
    public GameObject GameOverPanel;              // El panel completo que contiene todo el menú
    public Button botonReiniciar;              // Botón para continuar el juego
    public Button botonSalirMenu;             // Botón para volver al menú principal

    [Header("Configuración de Escenas")]
    public string nombreEscenaMenu = "UI Testing"; 
    public string nombreEscenaJuego = "GameScene"; // Nombre exacto de tu escena de menú principal

    [Header("Configuración de Audio")]
    public AudioClip gameOverAudioClip; // Arrastra aquí tu audio "Game Over (8-Bit Music)"

    private AudioManager audioManager;        // Referencia al AudioManager
    private AudioSource gameOverAudioSource; // AudioSource específico para el Game Over
    private bool isOver = false;  
    
    void Start()
    {
       // Encontramos el AudioManager que ya existe en tu juego
        audioManager = FindFirstObjectByType<AudioManager>();

        // Verificamos que tengamos todo lo necesario
        if (audioManager == null)
        {
            Debug.LogError("¡No se encontró AudioManager! Asegúrate de que esté en la escena.");
        }

        // Configurar el AudioSource para el Game Over
        ConfigurarAudioGameOver();

        // Configuramos todos los botones con sus sonidos correspondientes
        ConfigurarBotonesConAudio();

        GameOverPanel.SetActive(false);
        
        // Nos suscribimos al evento de muerte del jugador
        SubscribirEventosDeJugador();
    }
    
    void OnDestroy()
    {
        // Nos desuscribimos de los eventos al destruir el objeto
        DesuscribirEventosDeJugador();
    }

    void Update()
    {

    }

    /// <summary>
    /// Suscribirse a los eventos de muerte del jugador
    /// </summary>
    void SubscribirEventosDeJugador()
    {
        // Buscar el jugador y suscribirse a su evento de muerte
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            Health healthJugador = jugador.GetComponent<Health>();
            if (healthJugador != null)
            {
                healthJugador.onDeath.AddListener(MostrarGameOver);
                Debug.Log("GameOverManager: Conectado al evento de muerte del jugador");
            }
            else
            {
                Debug.LogError("GameOverManager: El jugador no tiene componente Health!");
            }
        }
        else
        {
            Debug.LogError("GameOverManager: No se encontró el jugador con tag 'Player'!");
        }
        
        // También nos suscribimos al evento del WaveManager por si el tiempo se agota
        WaveManager.OnWaveFailed += MostrarGameOver;
    }
    
    /// <summary>
    /// Desuscribirse de los eventos para evitar memory leaks
    /// </summary>
    void DesuscribirEventosDeJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            Health healthJugador = jugador.GetComponent<Health>();
            if (healthJugador != null)
            {
                healthJugador.onDeath.RemoveListener(MostrarGameOver);
            }
        }
        
        WaveManager.OnWaveFailed -= MostrarGameOver;
    }
    
    /// <summary>
    /// Mostrar la pantalla de Game Over
    /// </summary>
    public void MostrarGameOver()
    {
        if (isOver) return; // Evitar mostrar múltiples veces
        
        isOver = true;
        Debug.Log("GameOverManager: ¡Mostrando Game Over!");
        
        // PAUSAR TODO EL JUEGO COMPLETAMENTE
        PausarTodoElJuego();
        
        // Activar el panel de Game Over
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverManager: GameOverPanel no está asignado!");
        }
        
        // Reproducir sonido de Game Over si está disponible
        if (audioManager != null)
        {
            // Aquí puedes agregar un sonido específico de Game Over
            // audioManager.SonidoGameOver();
        }
        
        // Reproducir el audio específico de Game Over
        ReproducirAudioGameOver();
    }

    void ConfigurarBotonesConAudio()
    {
        // Configuramos cada botón con su funcionalidad y sonidos
        // Esto es similar a como lo hiciste en MainMenuManager

        // Botón Reanudar - simplemente quita la pausa
        botonReiniciar.onClick.AddListener(() =>
        {
            if (audioManager != null)
            {
                audioManager.SonidoClickBoton();  // Sonido de confirmación
            }
            
            // Reiniciar el juego correctamente
            ReiniciarJuego();
        });

        // Botón Salir al Menú - más complejo porque cambia de escena
        botonSalirMenu.onClick.AddListener(() => {
            if (audioManager != null)
            {
                audioManager.SonidoClickBoton();        // Sonido de clic
                audioManager.SonidoTransicion();        // Sonido de transición
                audioManager.ReproducirMusicaMenu();    // Cambiamos a música de menú
            }

            // Salir al menú correctamente
            SalirAlMenu();
        });

        // Añadimos sonidos de hover para mejor experiencia de usuario
        ConfigurarSonidosHover(botonReiniciar);
        ConfigurarSonidosHover(botonSalirMenu);
    }

    void ConfigurarSonidosHover(Button boton)
    {
        // Esta función añade el sonido cuando el mouse pasa sobre el botón
        // Es exactamente igual que en tu MainMenuManager

        UnityEngine.EventSystems.EventTrigger trigger = boton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        // Creamos el evento para cuando el mouse entra al botón
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
    
    /// <summary>
    /// Método manual para probar el Game Over (solo para testing)
    /// </summary>
    [ContextMenu("Test Game Over")]
    public void TestGameOver()
    {
        MostrarGameOver();
    }

    /// <summary>
    /// Método para probar el audio de Game Over (solo para testing)
    /// </summary>
    [ContextMenu("Test Game Over Audio")]
    public void TestAudioGameOver()
    {
        if (gameOverAudioSource != null && gameOverAudioClip != null)
        {
            if (gameOverAudioSource.isPlaying)
            {
                DetenerAudioGameOver();
            }
            else
            {
                ReproducirAudioGameOver();
            }
        }
        else
        {
            Debug.LogWarning("GameOverManager: Audio de Game Over no configurado para testing");
        }
    }

    /// <summary>
    /// Reiniciar el juego correctamente ocultando UI y reseteando estado
    /// </summary>
    void ReiniciarJuego()
    {
        Debug.Log("GameOverManager: Reiniciando juego...");
        
        // Detener el audio de Game Over
        DetenerAudioGameOver();
        
        // Reanudar todo antes de cambiar escena
        ReanudarTodoElJuego();
        
        // Ocultar el panel de Game Over
        GameOverPanel.SetActive(false);
        
        // Resetear el estado del manager
        isOver = false;
        
        // Recargar la escena del juego
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    /// <summary>
    /// Salir al menú principal correctamente ocultando UI y reseteando estado
    /// </summary>
    void SalirAlMenu()
    {
        Debug.Log("GameOverManager: Saliendo al menú principal...");
        
        // Detener el audio de Game Over
        DetenerAudioGameOver();
        
        // Reanudar todo antes de cambiar escena
        ReanudarTodoElJuego();
        
        // Ocultar el panel de Game Over
        GameOverPanel.SetActive(false);
        
        // Resetear el estado del manager
        isOver = false;

        // Cambiamos a la escena del menú principal
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    /// <summary>
    /// Resetear manualmente el estado del GameOver (útil para debugging)
    /// </summary>
    [ContextMenu("Reset GameOver State")]
    public void ResetearEstadoGameOver()
    {
        Debug.Log("GameOverManager: Reseteando estado manualmente");
        
        // Detener el audio de Game Over
        DetenerAudioGameOver();
        
        // Reanudar todo el juego
        ReanudarTodoElJuego();
        
        isOver = false;
        
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Pausar completamente todo el contenido del juego (enemigos, animaciones, etc.)
    /// </summary>
    void PausarTodoElJuego()
    {
        Debug.Log("GameOverManager: Pausando todo el contenido del juego...");
        
        // 1. Pausar el tiempo del juego (afecta a la mayoría de componentes)
        Time.timeScale = 0f;
        
        // 2. Pausar todos los enemigos individualmente
        PausarTodosLosEnemigos();
        
        // 3. Pausar animaciones que no dependan de Time.timeScale
        PausarAnimaciones();
        
        // 4. Pausar sistemas de audio del juego (no del UI)
        PausarAudioDelJuego();
        
        // 5. Detener el WaveManager si existe
        DetenerWaveManager();
    }
    
    /// <summary>
    /// Pausar todos los enemigos en la escena
    /// </summary>
    void PausarTodosLosEnemigos()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"GameOverManager: Pausando {enemigos.Length} enemigos");
        
        foreach (GameObject enemigo in enemigos)
        {
            // Pausar controladores de enemigos
            var controllers = enemigo.GetComponents<MonoBehaviour>();
            foreach (var controller in controllers)
            {
                if (controller != null)
                {
                    controller.enabled = false;
                }
            }
            
            // Pausar Rigidbody2D si existe
            Rigidbody2D rb = enemigo.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false; // Completamente pausar la física
            }
            
            // Pausar animadores
            Animator animator = enemigo.GetComponent<Animator>();
            if (animator != null)
            {
                animator.speed = 0f;
            }
        }
    }
    
    /// <summary>
    /// Pausar animaciones que podrían no estar afectadas por Time.timeScale
    /// </summary>
    void PausarAnimaciones()
    {
        // Pausar todos los animadores en la escena (incluyendo partículas)
        Animator[] animators = FindObjectsByType<Animator>(FindObjectsSortMode.None);
        foreach (Animator animator in animators)
        {
            if (animator.gameObject != GameOverPanel) // No pausar animaciones del UI de GameOver
            {
                animator.speed = 0f;
            }
        }
        
        // Pausar sistemas de partículas
        ParticleSystem[] particleSystems = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Pause();
        }
    }
    
    /// <summary>
    /// Pausar audio del juego (no del UI)
    /// </summary>
    void PausarAudioDelJuego()
    {
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in audioSources)
        {
            // Solo pausar audio que no sea del UI y que no sea nuestro Game Over audio
            if (!audioSource.transform.IsChildOf(transform) && 
                audioSource != gameOverAudioSource && 
                audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }
    
    /// <summary>
    /// Detener el WaveManager para evitar que continúe procesando
    /// </summary>
    void DetenerWaveManager()
    {
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.enabled = false;
            Debug.Log("GameOverManager: WaveManager pausado");
        }
    }

    /// <summary>
    /// Reanudar todo el contenido del juego cuando se reinicia
    /// </summary>
    void ReanudarTodoElJuego()
    {
        Debug.Log("GameOverManager: Reanudando todo el contenido del juego...");
        
        // 1. Restaurar el tiempo del juego
        Time.timeScale = 1f;
        
        // 2. Reanudar todos los enemigos
        ReanudarTodosLosEnemigos();
        
        // 3. Reanudar animaciones
        ReanudarAnimaciones();
        
        // 4. Reanudar audio del juego
        ReanudarAudioDelJuego();
        
        // 5. Reactivar el WaveManager
        ReactivarWaveManager();
    }
    
    /// <summary>
    /// Reanudar todos los enemigos en la escena
    /// </summary>
    void ReanudarTodosLosEnemigos()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemigo in enemigos)
        {
            // Reactivar controladores de enemigos
            var controllers = enemigo.GetComponents<MonoBehaviour>();
            foreach (var controller in controllers)
            {
                if (controller != null)
                {
                    controller.enabled = true;
                }
            }
            
            // Reactivar Rigidbody2D si existe
            Rigidbody2D rb = enemigo.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = true;
            }
            
            // Reanudar animadores
            Animator animator = enemigo.GetComponent<Animator>();
            if (animator != null)
            {
                animator.speed = 1f;
            }
        }
    }
    
    /// <summary>
    /// Reanudar animaciones
    /// </summary>
    void ReanudarAnimaciones()
    {
        // Reanudar todos los animadores
        Animator[] animators = FindObjectsByType<Animator>(FindObjectsSortMode.None);
        foreach (Animator animator in animators)
        {
            animator.speed = 1f;
        }
        
        // Reanudar sistemas de partículas
        ParticleSystem[] particleSystems = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }
    
    /// <summary>
    /// Reanudar audio del juego
    /// </summary>
    void ReanudarAudioDelJuego()
    {
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in audioSources)
        {
            // Solo reanudar audio que no sea del UI y que estaba pausado
            if (!audioSource.transform.IsChildOf(transform))
            {
                audioSource.UnPause();
            }
        }
    }
    
    /// <summary>
    /// Reactivar el WaveManager
    /// </summary>
    void ReactivarWaveManager()
    {
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.enabled = true;
            Debug.Log("GameOverManager: WaveManager reactivado");
        }
    }

    /// <summary>
    /// Configurar el AudioSource específico para el Game Over
    /// </summary>
    void ConfigurarAudioGameOver()
    {
        // Crear un AudioSource específico para el Game Over
        gameOverAudioSource = gameObject.AddComponent<AudioSource>();
        
        // Configurar el AudioSource
        gameOverAudioSource.clip = gameOverAudioClip;
        gameOverAudioSource.loop = true; // Para que se repita mientras esté el Game Over
        gameOverAudioSource.playOnAwake = false;
        gameOverAudioSource.volume = 0.7f; // Volumen moderado
        
        // Verificar que el clip esté asignado
        if (gameOverAudioClip == null)
        {
            Debug.LogWarning("GameOverManager: No se asignó el Game Over Audio Clip en el Inspector!");
        }
        else
        {
            Debug.Log($"GameOverManager: Audio de Game Over configurado: {gameOverAudioClip.name}");
        }
    }
    
    /// <summary>
    /// Reproducir el audio de Game Over
    /// </summary>
    void ReproducirAudioGameOver()
    {
        if (gameOverAudioSource != null && gameOverAudioClip != null)
        {
            Debug.Log("GameOverManager: Reproduciendo audio de Game Over");
            gameOverAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("GameOverManager: No se puede reproducir audio de Game Over - AudioSource o Clip no configurado");
        }
    }
    
    /// <summary>
    /// Detener el audio de Game Over
    /// </summary>
    void DetenerAudioGameOver()
    {
        if (gameOverAudioSource != null && gameOverAudioSource.isPlaying)
        {
            Debug.Log("GameOverManager: Deteniendo audio de Game Over");
            gameOverAudioSource.Stop();
        }
    }
}