using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Game.Services;

namespace Game.GameScene
{
    public abstract partial class UnitBase : MonoBehaviour, IInitialize
    {
        protected abstract Stats BaseStat { get; }

        public Stats maxStats;
        public Stats curStats;
        public Stats.Modifier modifier;
        public Effects effects;
        public SkillBase skill;

        public virtual void Initialize()
        {
            maxStats = new();
            curStats = new();
            modifier = new();
            effects = new(this);
            if (skill == null) skill = new SkillBase(this);

            maxStats.OnStatChanged += (key, oldvalue, newvalue) =>
            {
                var diff = newvalue - oldvalue;
                if (key == Stats.Key.Hp && diff < 0) return;
                if (key == Stats.Key.AttackDelay) return;

                curStats.SetStat(key, x => x + diff);
                curStats.SetStat(key, x => Mathf.Clamp(x, float.MinValue, maxStats.GetStat(key)));
            };

            modifier.OnValueChange += key => modifier.Calculate(key, maxStats, BaseStat);
        }
    }
}