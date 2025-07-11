using UnityEngine;

public class ShieldEffect : MonoBehaviour, IEffect
{
    public float duration = 5f;
    public float blinkTime = 1f; // Cuánto antes del final empieza a tintinear
    public float blinkInterval = 0.2f;

    private SpriteRenderer sprite;

    public void ApplyTo(GameObject target)
    {
        if (target.layer == LayerMask.NameToLayer("Enemies"))
            return;

        var receiver = target.GetComponent<DamageReceiver>() ?? target.GetComponentInParent<DamageReceiver>();
        if (receiver == null)
        {
            Debug.LogWarning("El objetivo no tiene DamageReceiver.");
            return;
        }

        receiver.ApplyShield();

        // ⚙️ Pegar efecto al objetivo
        transform.SetParent(target.transform);
        transform.localPosition = Vector3.zero;

        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            SetAlpha(0.4f); // Escudo visible pero semitransparente

        var runner = target.GetComponent<MonoBehaviour>();
        if (runner != null)
            runner.StartCoroutine(MonitorShield(receiver));
    }

    private System.Collections.IEnumerator MonitorShield(DamageReceiver receiver)
    {
        float elapsed = 0f;
        bool isBlinking = false;

        while (elapsed < duration && receiver.ShieldIsActive)
        {
            elapsed += Time.deltaTime;

            // Iniciar parpadeo si estamos en los últimos segundos
            if (!isBlinking && duration - elapsed <= blinkTime)
            {
                isBlinking = true;
                StartCoroutine(Blink());
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator Blink()
    {
        while (true)
        {
            SetAlpha(0.1f);
            yield return new WaitForSeconds(blinkInterval);
            SetAlpha(0.4f);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (sprite != null)
        {
            Color c = sprite.color;
            c.a = alpha;
            sprite.color = c;
        }
    }
}
