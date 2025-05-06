using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform pointLeft;
    [SerializeField] private Transform pointRigth;
    [SerializeField] private Transform _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(pointLeft.position, _player.position) < 1f)
        {
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearVelocity = new Vector2(-2f, 0);
            }
        }
        if (Vector2.Distance(pointRigth.position, _player.position) < 1f)
        {
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearVelocity = new Vector2(2f, 0);
            }
        }
    }
}
