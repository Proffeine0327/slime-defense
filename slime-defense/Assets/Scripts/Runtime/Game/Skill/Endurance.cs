using Game.Services;
using UniRx;
using UnityEngine;

namespace Game.GameScene
{
    public class Endurance : SkillBase
    {
        //services
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();

        public ReactiveProperty<int> wave = new();
        public bool removing;

        private Obstacle obstacle => caster as Obstacle;
        public override Sprite Icon => base.Icon;
        public override string Name => removing ? "철거 중" : "강인함";
        public override string Explain => 
            removing ? 
            $"이 장애물은 제거되고 있습니다. {wave.Value}웨이브 후에 제거됩니다." : 
            $"이 물체는 굳건하게 자리를 지키고 있습니다. 제거하는데 {wave.Value}웨이브의 시간이 필요합니다.";

        public Endurance(Obstacle caster) : base(caster)
        {
            wave.Subscribe(x =>
            {
                if(x <= 0 && obstacle.IsSelected)
                    selectManager.Select(null);
            });
        }

        public override void OnWaveEnd()
        {
            if(removing && wave.Value > 0)
                wave.Value--;
        }
    }
}