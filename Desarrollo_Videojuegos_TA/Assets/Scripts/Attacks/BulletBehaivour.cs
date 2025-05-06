using System.Collections;
using UnityEngine;

public class BulletBehaivour : MonoBehaviour
{
    [SerializeField] private int bulletForce; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.AddForce(new Vector2(bulletForce,0),ForceMode2D.Impulse);
        }

        StartCoroutine(Dissapear());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Dissapear()
    {
        
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        
    }
}
