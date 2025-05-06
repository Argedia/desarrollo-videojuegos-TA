using System;
using System.Drawing;
using UnityEngine;

public class Player_Movimiento : MonoBehaviour
{
    public float jumpForce = 5f;
    private bool isGrounded;
    [SerializeField] private GameObject point;

    [SerializeField] private Rigidbody2D rb;
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void Awake()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        PointMove();
    }

    private void PointMove()
    {
        CubeMove();
    }

    private void CubeMove()
    {
        if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = new Vector2(1.5f,rb.linearVelocity.y);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity =  new Vector2(-1.5f,rb.linearVelocity.y);
        }
    }
}
