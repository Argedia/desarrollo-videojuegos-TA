using System.Collections.Generic;
using UnityEngine;

public class PlatformInstance
{
    public GameObject instance;                 // instancia en el mundo
    public PlatformPiece data;                  // el SO que la definió
    public Vector2Int gridPos;                  // posición de inicio en la grilla
    public int remainingSpawns;                 // cuántos enemigos aún puede sostener
    public List<Vector2> enemySpawnPositions;   // coordenadas donde puede spawnear enemigos


    public Vector2 GetRandomSpawnPos()
{
    if (enemySpawnPositions == null || enemySpawnPositions.Count == 0)
        return instance.transform.position;

    int idx = Random.Range(0, enemySpawnPositions.Count);
    return enemySpawnPositions[idx];
}

}
