using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class DamageDealer : MonoBehaviour
{
    public enum DamageMode { Manual, Auto }

    [Header("Damage Settings")]
    [SerializeField] private DamageMode mode = DamageMode.Manual;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float hitCooldown = 0.5f;
    [SerializeField] private LayerMask damageableLayers;
    private int stackedBonusDamage = 0;
    private bool applyBonusOnce = false;

    private Collider2D col;
    private ContactFilter2D filter;
    private readonly List<Collider2D> results = new();
    private Dictionary<Collider2D, float> lastHitTimes = new();

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = damageableLayers,
            useTriggers = true
        };
    }

    private void Update()
    {
        if (mode == DamageMode.Auto)
        {
            TryDealDamage();
        }
    }


    /// <summary>
    /// Llamar manualmente desde la animación (modo Manual).
    /// </summary>
    public void TryDealDamage()
    {
        int totalDamage = damageAmount + ConsumeBonusDamage();


        results.Clear();
        col.Overlap(filter, results);

        foreach (var hit in results)
        {
            if (!hit) continue;

            if (mode == DamageMode.Auto)
            {
                if (lastHitTimes.TryGetValue(hit, out float lastHit) && Time.time - lastHit < hitCooldown)
                    continue;

                lastHitTimes[hit] = Time.time;
            }

            DamageReceiver receiver = hit.GetComponent<DamageReceiver>();
            if (receiver != null)
            {
                Vector2 hitOrigin = transform.position;
                receiver.ReceiveDamage(totalDamage, hitOrigin, knockbackForce);
            }
        }
    }

    public int ConsumeBonusDamage()
    {
        int bonus = stackedBonusDamage;
        stackedBonusDamage = 0;
        return bonus;
    }

    public int GetBonusDamage() => stackedBonusDamage;

    public void AddBonusDamage(int amount)
    {
        stackedBonusDamage += amount;
    }
}
