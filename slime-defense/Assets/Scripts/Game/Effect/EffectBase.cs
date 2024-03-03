public class EffectBase
{
    public UnitBase owner;

    public virtual void OnAdd() { }
    public virtual void OnRoundEnd() { }
    public virtual void OnRemove() { }
}