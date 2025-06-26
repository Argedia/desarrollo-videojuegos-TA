using UnityEngine;

[CreateAssetMenu(menuName = "WaveGen/EnemyType")]
public class EnemyType : ScriptableObject
{
    public string enemyName;
    public int cost;               // puntos de dificultad para colocarlo
    public GameObject prefab;
    public bool requiresPlatform;  // si necesita plataforma o puede flotar
}
