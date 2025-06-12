public interface IStatSource
{
    bool HasStat(StatType stat);
    float GetStat(StatType stat);
    void ModifyStat(StatType stat, float delta);
}
