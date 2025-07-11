using UnityEngine;

public class HealingEffect : MonoBehaviour, IEffect
{
    public int healAmmount = 1;

    public void ApplyTo(GameObject target)
    {
        if (target.layer == LayerMask.NameToLayer("Enemies"))
            return;
        var runner = target.GetComponent<MonoBehaviour>();
        if (runner != null)
            runner.StartCoroutine(Heal(target));
    }

    private System.Collections.IEnumerator Heal(GameObject target)
    {
        
        var health = target.GetComponent<Health>() ?? target.GetComponentInParent<Health>();
        if (health != null)
            Debug.Log("Te estan curando papito");
        var sprite = target.GetComponent<SpriteRenderer>();

        health.Heal(healAmmount);
        yield return new WaitForSeconds(healAmmount/10);
    }
}