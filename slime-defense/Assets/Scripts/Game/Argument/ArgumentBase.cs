using UnityEngine;

namespace Game.GameScene
{
    public class ArgumentBase
    {
        public virtual Sprite Icon { get; }
        public virtual string Title { get; }
        public virtual string Explain { get; }

        public virtual void OnUpdate() { }
        public virtual void OnSlimeUpdate(Slime[] slimes) { }
    }
}