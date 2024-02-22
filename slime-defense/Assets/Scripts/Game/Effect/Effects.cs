using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Effects
{
    public readonly Stats stats;
    public readonly StatModifier modifier;
    
    private Dictionary<string, EffectBase> container = new();
    public IReadOnlyDictionary<string, EffectBase> Container => container;

    public void AddEffect(string caster, EffectBase effect)
    {
        if(container.ContainsKey(caster)) return;

        effect.OnAdd();
        container.Add(caster, effect);
    }

    public Effects(Stats stats, StatModifier modifier)
    {
        this.stats = stats;
        this.modifier = modifier;
        ServiceProvider.Get<MonoBehaviourEvent>().OnUpdate += UpdateEffects;
    }

    ~Effects()
    {
        ServiceProvider.Get<MonoBehaviourEvent>().OnUpdate -= UpdateEffects;
    }

    public void UpdateEffects()
    {
        foreach(var e in container)
            e.Value.OnUpdate();
    }
}