using UnityEngine;

/// <summary>
/// Objeto que vive en la escena, anima, detecta colisión y aplica efectos.
/// </summary>
public class EffectObject : MonoBehaviour
{
    private IEffect[] effects;
    private Transform caster;

    public void Setup(Transform origin)
    {
        caster = origin;
        effects = GetComponents<IEffect>();

        // Aquí podrías reproducir animaciones o efectos visuales si quieres
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == caster) return;

        foreach (var effect in effects)
        {
            effect.ApplyTo(other.gameObject);
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        // Seguridad para autodestruir si no colisiona en cierto tiempo
        Destroy(gameObject, 5f);
    }
}
