using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;

    [SerializeField] private Transform _shootingPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            BulletShoot();
        }
    }

    void BulletShoot()
    {
        Instantiate(_bullet, _shootingPoint.position, _shootingPoint.rotation);
    }
}
