using UnityEngine;

namespace Game.GameScene
{
    public class ArgumentBase
    {
        public virtual Sprite Icon { get; }
        public virtual string Title => "베이스";
        public virtual string Explain => "베이스";

        public virtual void OnAdd() { }
        public virtual void OnUpdate() { }
        public virtual void OnSlimeUpdate() { }
    }
}