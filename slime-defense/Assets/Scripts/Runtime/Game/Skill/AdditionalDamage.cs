using System.Collections.Generic;
using UnityEngine;
using Game.Services;

namespace Game.GameScene
{
    public class AdditionalDamage : SkillBase
    {
        //service
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

        public AdditionalDamage(UnitBase caster) : base(caster) { }

        public override string Name => "ì¶”ê?? ê³µê²©!";
        public override string Explain => $"ê³µê²©?‹œ <color=#9d40db>{caster.curStats.GetStat(Stats.Key.AbilityPower) * 0.1f:#,##0.###}</color>ë§Œí¼?„ ì¶”ê??ë¡? ?ž…?ž™?‹ˆ?‹¤.";
        public override Sprite Icon => resourceLoader.skillIcons.GetValueOrDefault(nameof(AdditionalDamage));

        public override void OnAttack(UnitBase enemy)
        {
            base.OnAttack(enemy);
            enemy.curStats.SetStat(Stats.Key.Hp, x => x - caster.curStats.GetStat(Stats.Key.AbilityPower) * 0.1f);
        }
    }
}