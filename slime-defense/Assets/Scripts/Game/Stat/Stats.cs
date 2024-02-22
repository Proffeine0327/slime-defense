using System;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public enum Key
    {
        Hp,
        Speed,
        AttackDamage,
        AbilityPower,
        AttackRange,
        AttackDelay,

        Stun,
        Invincible,
        Stealth,
        AbsSight,
    }

    private Dictionary<Key, float> stats = new();

    public IReadOnlyDictionary<Key, float> _Stats => stats;

    /// <summary>
    /// return l - r
    /// </summary>
    /// <param name="l">left</param>
    /// <param name="r">right</param>
    /// <returns></returns>
    public static Stats GetDifference(Stats l, Stats r)
    {
        var diff = l.Clone();
        foreach(var rstat in r.stats)
            l.ModifyStat(rstat.Key, x => x - rstat.Value);
        return diff;
    }

    public float this[Key key]
    {
        get => GetStat(key);
        set => ModifyStat(key, x => value);
    }

    public float GetStat(Key key)
    {
        if (!stats.ContainsKey(key)) return 0;
        return stats[key];
    }

    public void ModifyStat(Key key, Func<float, float> modifier)
    {
        if (!stats.ContainsKey(key)) stats.Add(key, 0);
        stats[key] = modifier(stats[key]);
    }

    public bool RemoveStat(Key key)
    {
        return stats.Remove(key);
    }

    public bool AddStat(Key key, float value)
    {
        return stats.TryAdd(key, value);
    }

    public void ClearStat()
    {
        stats.Clear();
    }

    public Stats Clone()
    {
        return new Stats() { stats = new(stats) };
    }
}