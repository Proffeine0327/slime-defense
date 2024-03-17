using Game.Services;
using UnityEngine;

namespace Game.GameScene
{
    public class Test1 : ArgumentBase
    {
        //service
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        public override string Title => "목숨 회복";
        public override string Explain => "즉시 3의 라이프를 얻습니다";

        public override void OnAdd()
        {
            gameManager.SaveData.life += 3;
        }
    }
}