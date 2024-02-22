using UnityEngine;

public partial class UnitBase
{
    public abstract class SkillBase
    {
        protected readonly UnitBase caster;

        public virtual void OnAdd() { }
        public virtual void OnAttack(UnitBase unit) { }
        public virtual void OnRoundEnd() { }

        public SkillBase(UnitBase caster)
        {
            this.caster = caster;
        }
    }
}