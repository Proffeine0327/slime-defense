using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Services;

namespace Game.GameScene
{
    public class SkillBase : ISaveLoad
    {
        //service
        private GameManager gameManager => ServiceProvider.Get<GameManager>();
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

        public virtual Sprite Icon => resourceLoader.skillIcons.GetValueOrDefault(GetType().Name);
        public virtual string Name => "Skill_Base";
        public virtual string Explain => "Skill_Base_Name";

        protected readonly UnitBase caster;

        public static SkillBase GetSkill(string key, UnitBase caster)
        {
            key = "Game.GameScene." + key;
            if (string.IsNullOrEmpty(key)) return new SkillBase(caster);
            var t = Type.GetType(key);
            if (t == null) return new SkillBase(caster);

            return Activator.CreateInstance(t, args: caster) as SkillBase;
        }
        public virtual void OnAdd() { }
        public virtual void OnAttack(UnitBase enemy)
        {
            enemy.curStats.ModifyStat(Stats.Key.Hp, x => x - caster.curStats.GetStat(Stats.Key.AttackDamage));
        }
        public virtual void OnWaveEnd() { }
        public virtual void OnDamage(float damage)
        {
            caster.curStats.ModifyStat(Stats.Key.Hp, x => x - damage);
        }

        public SkillBase(UnitBase caster)
        {
            this.caster = caster;
            OnAdd();
            gameManager.OnWaveEnd += OnWaveEnd;
        }

        ~SkillBase()
        {
            gameManager.OnWaveEnd -= OnWaveEnd;
        }

        public string Save() { return string.Empty; }
        public void Load(string data) { }
    }
}