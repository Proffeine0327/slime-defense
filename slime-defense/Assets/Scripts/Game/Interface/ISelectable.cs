using UnityEngine;

namespace Game.GameScene
{
    public interface ISelectable
    {
        public Sprite Icon { get; }
        public int Lv { get; }
        public Stats DisplayStat { get; }
        public bool IsRemovable { get; }
        public int RemoveCost { get; }
        public string RemoveExplain { get; }

        public void OnRemove();
    }
}