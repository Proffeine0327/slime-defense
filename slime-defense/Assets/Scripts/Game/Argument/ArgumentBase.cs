using UnityEngine;

namespace Game.GameScene
{
    public class ArgumentBase
    {
        public virtual string Name { get; }

        public virtual void OnUpdate() { }

        public virtual void OnSlimeUpdate(Slime[] slimes) { }
    }
}