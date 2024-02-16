using System;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;

public class Stat
{
    private ReactiveDictionary<string, ReactiveProperty<float>> stats = new();

    public event Action<Dictionary<string, ReactiveProperty<float>>.ValueCollection> OnAnyStatChanged;

    public bool AddStat(string key, float value)
    {
        if(ContainsStat(key)) return false;

        var p = new ReactiveProperty<float>(value);
        p.Subscribe(_ => NotifyStatChanged());
        stats.Add(key, p);
        return true;
    }

    public bool ChangeStat(string key, float value)
    {
        if(!ContainsStat(key)) return false;
        stats[key].Value = value;
        return true;
    }

    public void AddOrChangeStat(string key, float value)
    {
        if(ContainsStat(key)) ChangeStat(key, value);
        else AddStat(key, value);
    }

    public bool RemoveStat(string key)
    {
        if(!ContainsStat(key)) return false;
        stats.Remove(key);
        return true;
    }

    public bool ContainsStat(string key)
    {
        return stats.ContainsKey(key);
    }

    public float GetStat(string key)
    {
        return stats[key].Value;
    }

    private void NotifyStatChanged()
    {
        OnAnyStatChanged?.Invoke(stats.Values);
    }

    public Stat()
    {
        stats.ObserveCountChanged().Subscribe(_ => NotifyStatChanged());
    }
}