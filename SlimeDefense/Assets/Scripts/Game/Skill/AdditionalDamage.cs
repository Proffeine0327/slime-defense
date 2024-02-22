using UnityEngine;

public class AdditionalDamage : UnitBase.SkillBase
{
    public AdditionalDamage(UnitBase caster) : base(caster) { }

    public override void OnAttack(UnitBase unit)
    {
        base.OnAttack(unit);
    }
}