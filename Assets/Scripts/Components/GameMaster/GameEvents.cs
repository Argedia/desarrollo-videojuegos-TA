using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<Health> OnEnemyDeath;
    public static event Action<Health, int> OnEnemyDamaged;

    public static void EnemyDied(Health enemy)
    {
        OnEnemyDeath?.Invoke(enemy);
    }

    public static void EnemyDamaged(Health enemy, int amount)
    {
        OnEnemyDamaged?.Invoke(enemy, amount);
    }
}
