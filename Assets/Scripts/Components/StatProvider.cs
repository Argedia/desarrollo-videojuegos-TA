using UnityEngine;
using System.Collections.Generic;
public class StatProvider : MonoBehaviour
{
    private List<IStatSource> sources = new();

    void Awake()
    {
        sources.AddRange(GetComponents<IStatSource>());
    }

    public float GetStat(StatType stat)
    {
        foreach (var s in sources)
            if (s.HasStat(stat)) return s.GetStat(stat);
        return 0f;
    }

    public void ModifyStat(StatType stat, float delta)
    {
        foreach (var s in sources)
            if (s.HasStat(stat))
                s.ModifyStat(stat, delta);
    }
}
