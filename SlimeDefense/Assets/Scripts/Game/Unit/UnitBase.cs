using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public abstract partial class UnitBase : MonoBehaviour
{
    protected Stats maxStats;
    protected Stats currentStats;
    protected StatModifier modifier;
    protected Effects effect;

    protected virtual void Awake()
    {
        maxStats = new();
        modifier = new();
        effect = new(maxStats, modifier);

        modifier.OnAnyValueChanged += CalculateStat;
    }

    public void CalculateStat()
    {
        var pre = maxStats.Clone();
        modifier.Calculate(maxStats);
        var diff = Stats.GetDifference(maxStats, pre);
        foreach (var v in diff._Stats)
        {
            if (v.Key == Stats.Key.Hp)
            {
                if (v.Value > 0)
                    currentStats.ModifyStat(v.Key, x => x + v.Value);
            }
            else
            {
                currentStats.ModifyStat(v.Key, x => x + v.Value);
            }
        }
    }
}