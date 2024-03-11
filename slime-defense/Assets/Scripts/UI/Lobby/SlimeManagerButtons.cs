using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Game.Services;
using UniRx;
using UnityEngine.SceneManagement;

namespace Game.UI.LobbyScene
{
    public class SlimeManagerButtons : MonoBehaviour
    {
        //service
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();
        private SceneNavigation sceneNavigation => ServiceProvider.Get<SceneNavigation>();

        [SerializeField] private Button deckButton;
        [SerializeField] private Button detailButton;

        private Sequence open;
        private Sequence close;

        private void Start()
        {
            open = DOTween
                .Sequence()
                .SetAutoKill(false)
                .Append((deckButton.transform as RectTransform).DOAnchorPosY(-50, 0.5f))
                .Insert(0.15f, (detailButton.transform as RectTransform).DOAnchorPosY(-50, 0.5f))
                .Pause();
            close = DOTween
                .Sequence()
                .SetAutoKill(false)
                .Append((detailButton.transform as RectTransform).DOAnchorPosY(100, 0.5f))
                .Insert(0, (deckButton.transform as RectTransform).DOAnchorPosY(100, 0.5f))
                .Pause();

            lobbyManager.IsSelectedStage.Subscribe(x =>
            {
                if (x)
                {
                    close.Complete();
                    open.Restart();
                }
                else
                {
                    open.Complete();
                    close.Restart();
                }
            });

            deckButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {

                });
            detailButton
                .OnClickAsObservable()
                .Subscribe(_ => sceneNavigation.LoadNewScene("SlimeDetail"));
        }
    }
}