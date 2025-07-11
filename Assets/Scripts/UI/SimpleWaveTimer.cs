using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleWaveTimer : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public Slider timerBar;
    public GameObject timerPanel;

    [Header("Colors")]
    public Color normalColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color criticalColor = Color.red;

    [Header("Thresholds")]
    public float warningThreshold = 10f;
    public float criticalThreshold = 5f;

    private WaveManager waveManager;
    private float currentTime;
    private float maxTime;

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();

        // Suscribirse a eventos
        WaveManager.OnWaveStarted += OnWaveStarted;
        WaveManager.OnWaveCompleted += OnWaveCompleted;
        WaveManager.OnWaveFailed += OnWaveFailed;
        WaveManager.OnRestStarted += OnRestStarted;

        // Ocultar inicialmente
        if (timerPanel != null)
            timerPanel.SetActive(false);
    }

    private void Update()
    {
        if (waveManager != null && waveManager.State == WaveState.InProgress)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(0, currentTime);

        // Actualizar texto
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }

        // Actualizar barra
        if (timerBar != null)
        {
            timerBar.value = currentTime / maxTime;
        }

        // Actualizar colores
        UpdateColors();
    }

    private void UpdateColors()
    {
        Color currentColor = normalColor;

        if (currentTime <= criticalThreshold)
            currentColor = criticalColor;
        else if (currentTime <= warningThreshold)
            currentColor = warningColor;

        if (timerBar != null)
        {
            // Cambiar color del Fill Area
            var fillRect = timerBar.fillRect;
            if (fillRect != null)
            {
                var image = fillRect.GetComponent<Image>();
                if (image != null)
                    image.color = currentColor;
            }
        }

        if (timerText != null)
            timerText.color = currentColor;
    }

    private void OnWaveStarted(int waveNumber)
    {
        maxTime = waveManager.waveTime;
        currentTime = maxTime;

        if (timerPanel != null)
            timerPanel.SetActive(true);

        UpdateTimer();
    }

    private void OnWaveCompleted(int waveNumber)
    {
        // Mantener visible pero parar actualización
    }

    private void OnWaveFailed()
    {
        currentTime = 0;
        UpdateTimer();
    }

    private void OnRestStarted(int waveNumber)
    {
        if (timerPanel != null)
            timerPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        WaveManager.OnWaveStarted -= OnWaveStarted;
        WaveManager.OnWaveCompleted -= OnWaveCompleted;
        WaveManager.OnWaveFailed -= OnWaveFailed;
        WaveManager.OnRestStarted -= OnRestStarted;
    }
}