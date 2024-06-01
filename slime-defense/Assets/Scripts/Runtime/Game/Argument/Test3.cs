using Game.Services;
using UnityEngine;

namespace Game.GameScene
{
    public class Test3 : ArgumentBase
    {
        //service
        private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();

        public override string Title => "우리는 하나";
        public override string Explain => "배치된 슬라임 하나당 주문력이 1% 증가합니다.";

        public override void OnAdd()
        {
            OnSlimeUpdate();
        }

        public override void OnSlimeUpdate()
        {
            foreach(var slime in slimeManager.Slimes)
            {
                slime.modifier.Set
                (
                    "Test3", 
                    Stats.Key.AbilityPower,
                    x => 0.01f * slimeManager.Slimes.Count,
                    x => 0
                );
            }
        }
    }
}