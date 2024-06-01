using Game.Services;
using UnityEngine;

namespace Game.GameScene
{
    public class Test1 : ArgumentBase
    {
        //service
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        public override string Title => "��� ȸ��";
        public override string Explain => "��� 3�� ü���� ȸ���մϴ�.";

        public override void OnAdd()
        {
            gameManager.SaveData.life += 3;
        }
    }
}