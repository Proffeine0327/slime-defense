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
        }

        private ReactiveDictionary<Key, float> stats = new();

        public event Action<Key, float, float> OnStatChanged;

        public Stats()
        {
            stats
                .ObserveAdd()
                .Subscribe(kvp => OnStatChanged?.Invoke(kvp.Key, 0, kvp.Value));
            stats
                .ObserveRemove()
                .Subscribe(kvp => OnStatChanged?.Invoke(kvp.Key, kvp.Value, 0));
            stats
                .ObserveReplace()
                .Where(kv => kv.OldValue != kv.NewValue)
                .Subscribe(kvp => OnStatChanged?.Invoke(kvp.Key, kvp.OldValue, kvp.NewValue));
        }

        public float this[Key key]
        {
            get => GetStat(key);
            set => ModifyStat(key, x => value);
        }

        public float GetStat(Key key)
        {
            if (!stats.ContainsKey(key)) return 0;
            return stats[key];
        }

        public IReadOnlyDictionary<Key, float> GetStats()
        {
            return new Dictionary<Key, float>(stats);
        }

        public void ModifyStat(Key key, Func<float, float> modifier)
        {
            if (!stats.ContainsKey(key)) stats.Add(key, 0);
            stats[key] = modifier(stats[key]);
        }

        public bool RemoveStat(Key key)
        {
            return stats.Remove(key);
        }

        public bool AddStat(Key key, float value)
        {
            return stats.TryAdd(key, value);
        }

        public void ClearStat()
        {
            foreach (var s in stats)
                stats.Remove(s.Key);
        }

        public Stats Clone()
        {
            var stats = new Stats();
            stats.ChangeFrom(this);
            return stats;
        }

        public void ChangeFrom(Stats stats)
        {
            foreach (var stat in stats.GetStats())
                ModifyStat(stat.Key, x => stat.Value);
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
            foreach(var stat in GetStats())
                sb.Append($"{stat.Key}\'{stat.Value}").Append(',');
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
                AddStat(key, value);
            }
        }
    }
}