using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Game.UI.GameScene
{
    public class GameEndDisplayer : MonoBehaviour
    {
        public enum GameEndSignalTarget { GameOver, GameClear }

        [System.Serializable]
        public struct FadeInfo
        {
            public enum MethodType { Append, Insert }

            public Graphic target;
            public Color color;
            public MethodType methodType;
        }

        //service
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        [SerializeField] private GameEndSignalTarget target;
        [SerializeField] private Image bg;
        [SerializeField] private FadeInfo[] fadeInfos;
        [SerializeField] private TextMeshProUGUI killAmount;
        [SerializeField] private TextMeshProUGUI wave;
        [SerializeField] private Button exit;

        private bool isDisplayed;

        private void Start()
        {
            exit
                .OnClickAsObservable()
                .Subscribe(_ => gameManager.RemoveGame());
        }

        private void Update()
        {
            switch (target)
            {
                case GameEndSignalTarget.GameOver: if(!gameManager.IsGameOver) return; break;
                case GameEndSignalTarget.GameClear: if(!gameManager.IsGameClear) return; break;
            }
            if (isDisplayed) return;

            isDisplayed = true;

            killAmount.text = gameManager.SaveData.killAmount.ToString("#,##0");
            wave.text = gameManager.SaveData.wave.ToString("#,##0");

            bg.color = default;
            foreach (var i in fadeInfos)
                i.target.color = new Color(i.color.r, i.color.g, i.color.b, 0);

            var time = 0.5f;
            var seq = DOTween
                .Sequence()
                .SetUpdate(true)
                .OnStart(() => bg.gameObject.SetActive(true))
                .Append(bg.DOColor(new Color(0, 0, 0, 0.5f), time).SetUpdate(true));

            var fTime = 0f;
            foreach (var i in fadeInfos)
            {
                switch (i.methodType)
                {
                    case FadeInfo.MethodType.Append:
                        seq.Append(i.target.DOColor(i.color, time).SetUpdate(true));
                        fTime += time;
                        break;
                    case FadeInfo.MethodType.Insert:
                        seq.Insert(fTime, i.target.DOColor(i.color, time).SetUpdate(true));
                        break;
                }
            }
            seq.Play();
        }
    }
}
