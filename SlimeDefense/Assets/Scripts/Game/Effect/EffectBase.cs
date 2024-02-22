public class EffectBase
{
    public Effects owner;

    public virtual void OnAdd() { }
    public virtual void OnUpdate() { }
    public virtual void OnRoundEnd() { }
    public virtual void OnRemove() { }
}