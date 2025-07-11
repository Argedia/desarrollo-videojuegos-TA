using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public List<PlatformPiece> platformLibrary;
    public int gridWidth, gridHeight;
    public float cellWidth;
    public float cellHeight;

    private bool[,] grid;
    private List<PlatformInstance> spawnedPlatforms;

    private void Awake()
    {
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
                Debug.Log("Quedan puntos de plataformas:" + points);
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
        Vector2 worldOrigin = new Vector2(-gridWidth * cellWidth / 2f, -gridHeight * cellHeight / 2f);
        Vector2 centerOffset = new Vector2(size.x * cellWidth, size.y * cellHeight) / 2f;
        return worldOrigin + new Vector2(gridPos.x * cellWidth, gridPos.y * cellHeight) + centerOffset;
    }



    public List<PlatformInstance> GetSpawnablePlatforms() => spawnedPlatforms;

    // Necesitas este método para que funcione correctamente
    private Vector2Int? TryPlace(PlatformPiece piece)
    {
        int attempts = 50;

        while (attempts-- > 0)
        {
            // Regla 1: si no hay plataformas todavía → colocar abajo
            if (spawnedPlatforms.Count == 0)
            {
                int x = Random.Range(0, gridWidth - piece.size.x + 1);
                int y = 0; // capa más baja
                if (CanPlaceAt(x, y, piece.size))
                    return new Vector2Int(x, y);
            }
            else
            {
                // Regla 2: colocar cerca de plataformas existentes
                foreach (var platform in spawnedPlatforms)
                {
                    Vector2Int basePos = platform.gridPos;
                    Vector2Int size = platform.data.size;

                    for (int dx = -1; dx <= size.x; dx++) // permite -1 (izq) y +size (der)
                    {
                        int x = basePos.x + dx;
                        int y = basePos.y + size.y; // justo encima

                        if (x < 0 || x + piece.size.x > gridWidth) continue;
                        if (y < 0 || y + piece.size.y > gridHeight) continue;

                        if (CanPlaceAt(x, y, piece.size))
                            return new Vector2Int(x, y);
                    }
                }
            }
        }

        return null; // No se pudo
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

    public void ClearPlatforms()
    {
        if (spawnedPlatforms == null)
        {
            Debug.Log("No encontré plataformas!");
        }

        foreach (var platform in spawnedPlatforms)
        {
            if (platform.instance != null)
                Destroy(platform.instance);
        }

        spawnedPlatforms.Clear();
        grid = new bool[gridWidth, gridHeight]; // resetear grilla
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (grid == null)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 center = GridToWorld(new Vector2Int(x, y), Vector2Int.one);
                    Gizmos.DrawWireCube(center, new Vector3(cellWidth, cellHeight, 0.1f));
                }
            }
        }

        else
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y])
                    {
                        Vector3 center = GridToWorld(new Vector2Int(x, y), Vector2Int.one);
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(center, new Vector3(cellWidth * 0.9f, cellHeight * 0.9f, 0.1f));
                    }
                }
            }
        }

    }

}
