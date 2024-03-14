using System;
using UnityEngine;

namespace Game.GameScene
{
    public interface ISelectable
    {
        public int Lv { get; }
        public bool IsRemovable { get; }
        public float RemoveCost { get; }
        public string RemoveExplain { get; }
        public Sprite Icon { get; }
        public Stats DisplayStat { get; }
        public SkillBase Skill { get; }

        public void OnRemove();
    }
}