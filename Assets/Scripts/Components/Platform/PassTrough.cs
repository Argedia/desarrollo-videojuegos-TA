using System.Collections;
using UnityEngine;

public class PlatformPassThrough : MonoBehaviour
{
    private Collider2D platformCollider;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    public void IgnorePlayerTemporarily(Collider2D playerCollider)
    {
        if (platformCollider != null && playerCollider.CompareTag("Player"))
        {
            StartCoroutine(IgnoreTemporarily(playerCollider));
        }
    }

    private IEnumerator IgnoreTemporarily(Collider2D playerCollider)
    {
        Physics2D.IgnoreCollision(platformCollider, playerCollider, true);
        yield return new WaitForSeconds(1f);
        Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
    }
}