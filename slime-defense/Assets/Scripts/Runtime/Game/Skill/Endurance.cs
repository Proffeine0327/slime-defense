using UniRx;
using UnityEngine;

namespace Game.GameScene
{
    public class Endurance : SkillBase
    {
        public ReactiveProperty<int> wave = new();
        public bool removing;

        public override Sprite Icon => base.Icon;
        public override string Name => removing ? "철거 중" : "강인함";
        public override string Explain => 
            removing ? 
            $"이 장애물은 제거되고 있습니다. {wave.Value}웨이브 후에 제거됩니다." : 
            $"이 물체는 굳건하게 자리를 지키고 있습니다. 제거하는데 {wave.Value}웨이브의 시간이 필요합니다.";

        public Endurance(UnitBase caster) : base(caster) { }

        public override void OnWaveEnd()
        {
            if(removing)
                wave.Value--;
        }
    }
}