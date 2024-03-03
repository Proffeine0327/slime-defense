using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase
{
    //service
    private GameManager gameManager => ServiceProvider.Get<GameManager>();

    public virtual string Name { get; }
    public virtual string Explain { get; }
    public virtual Sprite Icon { get; }

    protected readonly UnitBase caster;

    public static SkillBase GetSkill(string key, UnitBase caster)
    {
        if(string.IsNullOrEmpty(key)) return new SkillBase(caster);
        var t = Type.GetType(key);
        if (t == null) return new SkillBase(caster);
        return Activator.CreateInstance(t, args: caster) as SkillBase;
    }

    public virtual void OnAdd()
    {

    }

    public virtual void OnAttack(UnitBase enemy)
    {
        enemy.curStats.ModifyStat(Stats.Key.Hp, x => x - caster.curStats.GetStat(Stats.Key.AttackDamage));
    }

    public virtual void OnRoundEnd()
    {

    }

    public virtual void OnDamage(float damage)
    {
        caster.curStats.ModifyStat(Stats.Key.Hp, x => x - damage);
    }

    public SkillBase(UnitBase caster)
    {
        this.caster = caster;
        OnAdd();
        gameManager.OnWaveEnd += OnRoundEnd;
    }

    ~SkillBase()
    {
        gameManager.OnWaveEnd -= OnRoundEnd;
    }
}
