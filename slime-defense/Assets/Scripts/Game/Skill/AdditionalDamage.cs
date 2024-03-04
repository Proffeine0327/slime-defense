using System.Collections.Generic;
using UnityEngine;

public class AdditionalDamage : SkillBase
{
    //service
    private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

    public AdditionalDamage(UnitBase caster) : base(caster) { }

    public override string Name => "추가 공격!";
    public override string Explain => $"공격시 <color=#9d40db>{caster.curStats.GetStat(Stats.Key.AbilityPower) * 0.1f}</color>만큼을 추가로 입힙니다.";
    public override Sprite Icon => resourceLoader.skillIcons.GetValueOrDefault(nameof(AdditionalDamage));

    public override void OnAttack(UnitBase enemy)
    {
        base.OnAttack(enemy);
        enemy.curStats.ModifyStat(Stats.Key.Hp, x => x - caster.curStats.GetStat(Stats.Key.AbilityPower) * 0.1f);
    }
}