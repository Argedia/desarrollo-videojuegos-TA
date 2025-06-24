using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    public int waveNumber;
    public List<GameObject> enemyPrefabs; // Tipos de enemigos
    public float timeLimit;             // Tiempo para completar la oleada
}
