using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public abstract partial class UnitBase : MonoBehaviour
{
    protected abstract Stats BaseStats { get; }

    public Stats maxStats;
    public Stats curStats;
    public Stats.Modifier modifier;
    public Effects effects;
    public SkillBase skill;

    protected virtual void Start()
    {
        maxStats = new();
        curStats = new();
        modifier = new();
        effects = new(this);
        if (skill == null) skill = new SkillBase(this);

        maxStats.OnStatChanged += (key, oldvalue, newvalue) =>
        {
            curStats.ModifyStat(key, x => Mathf.Clamp(x, float.MinValue, maxStats.GetStat(key)));

            var diff = newvalue - oldvalue;
            if (key == Stats.Key.Hp && diff < 0) return;
            if (key == Stats.Key.AttackDelay) return;

            curStats.ModifyStat(key, x => x + diff);
        };

        modifier.OnValueChange += key => modifier.Calculate(key, maxStats, BaseStats);
    }
}