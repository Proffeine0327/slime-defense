using System;
using System.Collections.Generic;
using UniRx;

public class Effect
{
    public class EffectInfo
    {
        public ReactiveProperty<string> stat = new();
        public ReactiveProperty<int> round = new();
        public ReactiveProperty<float> time = new();
        public ReactiveProperty<float> value = new();

        public bool IsEnd => round.Value != -1 && time.Value < 0 && round.Value < 0;

        public EffectInfo(Effect effects)
        {
            stat.Subscribe(_ => effects.NotifyEffectChanged());
            round.Subscribe(_ => effects.NotifyEffectChanged());
            time.Subscribe(_ => effects.NotifyEffectChanged());
            value.Subscribe(_ => effects.NotifyEffectChanged());
        }
    }

    //caster, effect
    private ReactiveDictionary<string, EffectInfo> effects = new();

    public event Action<Dictionary<string, EffectInfo>.ValueCollection> OnAnyEffectChanged;

    private void NotifyEffectChanged()
    {
        OnAnyEffectChanged?.Invoke(effects.Values);
    }

    public Effect()
    {
        effects.ObserveCountChanged().Subscribe(_ => NotifyEffectChanged());
    }
}