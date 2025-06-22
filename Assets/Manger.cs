using UnityEngine;

public class MangerMinions : MonoBehaviour
{
    public static MangerMinions Instance;

    public int minionsKilled;
    //
    // art is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ManagerMinions.Instance.minionsKilled++;
    }
}
