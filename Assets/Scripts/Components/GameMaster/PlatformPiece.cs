using UnityEngine;

[CreateAssetMenu(menuName = "WaveGen/PlatformPiece")]
public class PlatformPiece : ScriptableObject
{
    public string pieceName;
    public Vector2Int size;        // tamaño en celdas
    public int cost;               // puntos de dificultad
    public GameObject prefab;
    public int maxEnemies;         // cuántos enemigos pueden ir encima
}
