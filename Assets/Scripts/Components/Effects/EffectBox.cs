using UnityEngine;

public class EffectBox : MonoBehaviour
{
    public Effect effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        effect.ApplyTo(collision.gameObject);
    }
}
