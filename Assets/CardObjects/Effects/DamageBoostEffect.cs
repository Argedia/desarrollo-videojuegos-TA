using UnityEngine;

public class DamageBoostEffect : MonoBehaviour, IEffect
{
    public int bonusDamage = 1;

    private Transform followTarget;

    public void ApplyTo(GameObject target)
    {
        if (target.layer == LayerMask.NameToLayer("Enemies"))
            return;

        var dealer = target.GetComponentInChildren<DamageDealer>(true);
        if (dealer == null)
        {
            Debug.LogWarning("El objetivo no tiene DamageDealer.");
            return;
        }

        dealer.AddBonusDamage(bonusDamage);

        var runner = target.GetComponent<MonoBehaviour>();
        if (runner != null)
        {
            followTarget = target.transform;
            runner.StartCoroutine(MonitorBuff(dealer));
            runner.StartCoroutine(FollowTarget());
        }
    }

    private System.Collections.IEnumerator MonitorBuff(DamageDealer dealer)
    {
        while (dealer.GetBonusDamage() > 0)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator FollowTarget()
    {
        while (followTarget != null)
        {
            transform.position = followTarget.position + Vector3.up; // Puedes ajustar el offset si quieres
            yield return null;
        }
    }
}