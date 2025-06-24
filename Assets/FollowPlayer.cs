using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0); // por encima del personaje

    void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;
    }
}
