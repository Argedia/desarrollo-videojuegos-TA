using System.Drawing;
using UnityEngine;

public class Player_Movimiento : MonoBehaviour
{
    public float jumpForce = 5f;
    private bool isGrounded;
    [SerializeField] private GameObject point;
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
        if (Input.GetKey(KeyCode.W))
        {
            point.transform.position = new Vector2(0, 0.1f) + point.transform.position;
        }
        if (Input.GetKey(KeyCode.S))
        {
            point.transform.position.y += -0.1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            point.transform.position = new Vector2(0.1f, 0) + point.transform.position;
        }
        if (Input.GetKey(KeyCode.A))
        {
            point.transform.position = new Vector2(-0.1f, 0) + point.transform.position;
        }

        CubeMove();
    }

    private void CubeMove()
    {
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = (point.transform.position - transform.position) * 1.5f;
        }
    }
}
