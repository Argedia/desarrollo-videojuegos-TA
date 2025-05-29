using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    [System.Serializable]
    public class PlatformSpawnData
    {
        public GameObject prefab; // Prefab de la plataforma
        public Vector2 position;  // Dónde aparecerá
        public bool initiallyActive = true; // Si está activa desde el inicio
    }

    public List<PlatformSpawnData> allPlatforms; // Lista editable desde el Inspector

    private List<GameObject> spawnedPlatforms = new List<GameObject>();

    public void SpawnInitialPlatforms()
    {
        foreach (var data in allPlatforms)
        {
            GameObject platform = Instantiate(data.prefab, data.position, Quaternion.identity);
            platform.SetActive(data.initiallyActive);
            spawnedPlatforms.Add(platform);
        }
    }

    public void EnablePlatform(int index)
    {
        if (index >= 0 && index < spawnedPlatforms.Count)
            spawnedPlatforms[index].SetActive(true);
    }

    public void DisablePlatform(int index)
    {
        if (index >= 0 && index < spawnedPlatforms.Count)
            spawnedPlatforms[index].SetActive(false);
    }

    public void ClearAll()
    {
        foreach (var plat in spawnedPlatforms)
        {
            Destroy(plat);
        }
        spawnedPlatforms.Clear();
    }
}
