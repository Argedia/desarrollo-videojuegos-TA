using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;
public class GoDown : MonoBehaviour
{
    private bool flag = false;
    [SerializeField] private GameObject Suelo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (flag)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("COlISIONNN");
                //Debug.Log("GAAAAAAAA");

                //var boxCol = gameObject.GetComponent<BoxCollider2D>();
                //boxCol.excludeLayers = 3;
                //StartCoroutine(comeBack(boxCol));

                if (TryGetComponent<BoxCollider2D>(out BoxCollider2D boxCollider2D))
                {
                    //Debug.Log("GAAAAAAAA22");
                    //boxCollider2D.excludeLayers = groundLayer;
                    //StartCoroutine(comeBack(boxCollider2D));
                    if (MathF.Abs(Suelo.transform.position.y - gameObject.transform.position.y) > 2)
                    {
                        Debug.Log("GAAAAAAAA22");
                        boxCollider2D.isTrigger = true;
                        //StartCoroutine(comeBack(boxCollider2D));
                    }

                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        flag = true;


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        flag = false;
    }

    private void OnTriggerEnter2D(Collider2D suelo)
    {
        if (suelo.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            var boxCol = gameObject.GetComponent<BoxCollider2D>();
            boxCol.isTrigger = false;
        } 
    }

    private IEnumerator comeBack(BoxCollider2D boxCol)
    {
        yield return new WaitForSeconds(1f);
        boxCol.isTrigger = false;

    }
}
