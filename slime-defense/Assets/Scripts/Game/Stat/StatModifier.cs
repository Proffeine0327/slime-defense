using System;
using System.Collections.Generic;
using UniRx;

public class StatModifier
{
    public enum Sign { Additive, Percentage }
    public class Info
    {
        private readonly StatModifier owner;

        private Stats.Key key;
        private float value;

        public Stats.Key Key => key;
        public float Value
        {
            get => value;
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    owner.valueChanged = true;
                }
            }
        }

        public Info(StatModifier owner, Stats.Key key, float value)
        {
            this.owner = owner;
            this.key = key;
            this.value = value;
        }
    }

    private Dictionary<string, Info> percentageInfo = new();
    private Dictionary<string, Info> additiveInfo = new();
    private bool valueChanged;

    public event Action OnAnyValueChanged;

    public StatModifier()
    {
        percentageInfo.ObserveEveryValueChanged(c => c.Count).Subscribe(_ => valueChanged = true);
        ServiceProvider.Get<MonoBehaviourEvent>().OnUpdate -= ObserveValueChange;
    }

    ~StatModifier()
    {
        ServiceProvider.Get<MonoBehaviourEvent>().OnUpdate -= ObserveValueChange;
    }

    public void ObserveValueChange()
    {
        if(valueChanged)
        {
            valueChanged = false;
            OnAnyValueChanged?.Invoke();
        }
    }

    public void AddOrChangeModifyInfo
    (
        string caster,
        Stats.Key key = default,
        Sign sign = default,
        float value = 0
    )
    {
        var info = new Info(this, key, value);

        switch (sign)
        {
            case Sign.Additive:
                if (additiveInfo.ContainsKey(caster))
                {
                    additiveInfo[caster] = info;
                    valueChanged = true;
                    return;
                }

                additiveInfo.Add(caster, info);
                break;
            case Sign.Percentage:
                if (percentageInfo.ContainsKey(caster))
                {
                    percentageInfo[caster] = info;
                    valueChanged = true;
                    return;
                }

                percentageInfo.Add(caster, info);
                break;
        }
    }

    public void RemoveStat(string caster)
    {
        if(additiveInfo.Remove(caster))
        {

        }
        percentageInfo.Remove(caster);
    }

    public void Calculate(Stats stat)
    {
        var sum = new Dictionary<Stats.Key, float>();
        foreach(var i in percentageInfo)
        {
            if(sum.ContainsKey(i.Value.Key)) sum[i.Value.Key] += i.Value.Value;
            else sum.Add(i.Value.Key, i.Value.Value);
        }
        foreach(var s in sum) stat.ModifyStat(s.Key, x => x + x * s.Value);
        sum.Clear();
        foreach(var i in additiveInfo)
        {
            if(sum.ContainsKey(i.Value.Key)) sum[i.Value.Key] += i.Value.Value;
            else sum.Add(i.Value.Key, i.Value.Value);
        }
        foreach(var s in sum) stat.ModifyStat(s.Key, x => x + s.Value);
    }
}