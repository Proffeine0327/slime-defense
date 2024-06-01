using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UniRx;
using System.Collections;

namespace Game.GameScene
{
    public partial class Stats : ISaveLoad
    {
        //define
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

            End
        }

        //field
        private ReactiveCollection<float> stats;

        //event
        /// <summary>
        /// key, old, new
        /// </summary>
        public event Action<Key, float, float> OnStatChanged;

        //method
        public Stats()
        {
            stats = new();
            for (int i = 0; i < (int)Key.End; i++)
                stats.Add(0);

            stats
                .ObserveReplace()
                .Subscribe(e => OnStatChanged?.Invoke((Key)e.Index, e.OldValue, e.NewValue));
        }

        public float this[Key key]
        {
            get => GetStat(key);
            set => SetStat(key, x => value);
        }

        public float GetStat(Key key)
        {
            return stats[(int)key];
        }

        public IReadOnlyList<float> GetStats()
        {
            return stats;
        }

        public void SetStat(Key key, Func<float, float> modifier)
        {
            stats[(int)key] = modifier(stats[(int)key]);
        }

        public Stats Clone()
        {
            var stats = new Stats();
            stats.ChangeFrom(this);
            return stats;
        }

        public void ChangeFrom(Stats target)
        {
            var stats = target.GetStats();
            for (int i = 0; i < stats.Count; i++)
                SetStat((Key)i, x => stats[i]);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var stat in stats)
                sb.Append(stat.ToString()).Append('\n');
            return sb.ToString();
        }

        public string Save()
        {
            var sb = new StringBuilder();
            for(int i = 0; i < (int)Key.End; i++)
                sb.Append($"{(Key)i}\'{GetStat((Key)i)}").Append(',');
            return sb.ToString();
        }

        public void Load(string data)
        {            
            var split = data.Split(',');
            foreach(var s in split)
            {
                if(string.IsNullOrEmpty(s)) continue;
                
                var kvp = s.Split('\'');
                var key = Enum.Parse<Key>(kvp[0]);
                var value = float.Parse(kvp[1]);

                SetStat(key, x => value);
            }
        }
    }
}