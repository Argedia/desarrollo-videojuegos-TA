using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    [System.Serializable]
    public class PlatformSpawnData
    {
        public GameObject prefab; // Prefab de la plataforma
        public Vector2 position;  // D�nde aparecer�
        public bool initiallyActive = true; // Si est� activa desde el inicio
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
    
    
    public void OnWave(int waveNumber)
    {
        switch (waveNumber)
        {
            case 1:
                EnablePlatform(6);
                EnablePlatform(7);

                DisablePlatform(0);
                break;

            case 2:
                break;

            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            default:
                ClearAll();
                break;
        }
    }
}
