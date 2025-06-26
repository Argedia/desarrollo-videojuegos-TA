using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public List<PlatformPiece> platformLibrary;
    public int gridWidth = 6, gridHeight = 3;
    public float cellWidth;
    public float cellHeight;

    private bool[,] grid;
    private List<PlatformInstance> spawnedPlatforms;

    public float totalWidth = 30f;   // ancho del área total donde irá el grid
    public float totalHeight = 15f;  // alto del área total

    private void Awake()
    {
        cellWidth = totalWidth / gridWidth;
        cellHeight = totalHeight / gridHeight;
    }

    public void GeneratePlatforms(int points, int requiredEnemySlots)
    {
        grid = new bool[gridWidth, gridHeight];
        spawnedPlatforms = new List<PlatformInstance>();
        int currentSlots = 0;
        int attempts = 0;

        while ((points > 0 || currentSlots < requiredEnemySlots) && attempts < 100)
        {
            var piece = platformLibrary[Random.Range(0, platformLibrary.Count)];
            if (piece.cost > points && currentSlots >= requiredEnemySlots)
            {
                attempts++;
                continue;
            }

            var pos = TryPlace(piece);
            if (pos != null)
            {
                SpawnPlatform(piece, pos.Value);
                points -= piece.cost;
                currentSlots += piece.maxEnemies;
            }

            attempts++;
        }

        if (currentSlots < requiredEnemySlots)
            Debug.LogWarning("¡No se pudo generar suficiente suelo para todos los enemigos!");
    }

    private void SpawnPlatform(PlatformPiece piece, Vector2Int gridPos)
    {
        // 1. Calcular posición en el mundo centrada
        Vector3 worldPos = GridToWorld(gridPos, piece.size);

        // 2. Instanciar el prefab
        GameObject instance = Instantiate(piece.prefab, worldPos, Quaternion.identity, transform);

        // 3. Marcar la grilla como ocupada
        for (int dx = 0; dx < piece.size.x; dx++)
        {
            for (int dy = 0; dy < piece.size.y; dy++)
            {
                grid[gridPos.x + dx, gridPos.y + dy] = true;
            }
        }

        // 4. Calcular posiciones de spawn de enemigos con desfase vertical (no celda arriba)
        List<Vector2> enemySpots = new();
        for (int dx = 0; dx < piece.size.x; dx++)
        {
            Vector2Int cell = new Vector2Int(gridPos.x + dx, gridPos.y + piece.size.y - 1);
            Vector2 center = GridToWorld(cell, Vector2Int.one);
            enemySpots.Add(center + Vector2.up * 0.5f); // desplazamiento vertical ligero
        }

        // 5. Crear instancia de plataforma
        var platform = new PlatformInstance
        {
            instance = instance,
            data = piece,
            gridPos = gridPos,
            remainingSpawns = piece.maxEnemies,
            enemySpawnPositions = enemySpots
        };

        spawnedPlatforms.Add(platform);
    }

    private Vector3 GridToWorld(Vector2Int gridPos, Vector2Int size)
    {
        Vector2 worldOrigin = new Vector2(-totalWidth / 2f, -totalHeight / 2f);
        Vector2 centerOffset = new Vector2(size.x * cellWidth, size.y * cellHeight) / 2f;
        return worldOrigin + new Vector2(gridPos.x * cellWidth, gridPos.y * cellHeight) + centerOffset;
    }


    public List<PlatformInstance> GetSpawnablePlatforms() => spawnedPlatforms;

    // Necesitas este método para que funcione correctamente
    private Vector2Int? TryPlace(PlatformPiece piece)
    {
        int attempts = 20;
        while (attempts-- > 0)
        {
            int x = Random.Range(0, gridWidth - piece.size.x + 1);
            int y = Random.Range(0, gridHeight - piece.size.y + 1);
            if (CanPlaceAt(x, y, piece.size))
                return new Vector2Int(x, y);
        }
        return null;
    }

    private bool CanPlaceAt(int x, int y, Vector2Int size)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dy = 0; dy < size.y; dy++)
            {
                if (grid[x + dx, y + dy]) return false;
            }
        }
        return true;
    }
    
    private void OnDrawGizmos()
{
    Gizmos.color = Color.green;
    for (int x = 0; x < gridWidth; x++)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            Vector3 center = GridToWorld(new Vector2Int(x, y), Vector2Int.one);
            Gizmos.DrawWireCube(center, new Vector3(cellWidth, cellHeight, 0.1f));
        }
    }
}

}
