using Game.Services;
using UnityEngine;

namespace Game.GameScene
{
    public class Test3 : ArgumentBase
    {
        //service
        private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();

        public override string Title => "�츮�� �ϳ�";
        public override string Explain => "��ġ�� ������ �ϳ��� �ֹ����� 1% �����մϴ�.";

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