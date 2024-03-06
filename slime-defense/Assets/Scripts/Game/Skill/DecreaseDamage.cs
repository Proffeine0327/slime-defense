using System.Collections.Generic;
using UnityEngine;
using Game.Services;

namespace Game.GameScene
{
    public class DecreaseDamage : SkillBase
    {
        //service
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

        public DecreaseDamage(UnitBase caster) : base(caster) { }

        public override string Name => "피해 감소";
        public override string Explain => $"받는 피해가 <color=#9d40db>{caster.curStats.GetStat(Stats.Key.AbilityPower) * 0.1f}</color>감소합니다.";
        public override Sprite Icon => resourceLoader.skillIcons.GetValueOrDefault(nameof(DecreaseDamage));

        public override void OnDamage(float damage)
        {
            base.OnDamage(damage - caster.curStats.GetStat(Stats.Key.AbilityPower) * 0.1f);
        }
    }
}