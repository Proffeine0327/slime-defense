using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using DG.Tweening;

namespace Game.UI.LobbyScene
{
    public class StageInfoRollAnimation : MonoBehaviour
    {
        //service
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();
        
        //member
        private Sequence openSeq;
        private Sequence closeSeq;

        //property
        private RectTransform rectTransform => transform as RectTransform;

        private void Start()
        {
            openSeq = DOTween
                        .Sequence()
                        .SetAutoKill(false)
                        .Append(rectTransform.DOAnchorPosX(-450, 0.33f).SetEase(Ease.Unset).SetUpdate(true))
                        .Append(rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 820), 0.33f).SetEase(Ease.OutQuart).SetUpdate(true))
                        .Pause();
            closeSeq = DOTween
                        .Sequence()
                        .SetAutoKill(false)
                        .Append(rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 1), 0.33f).SetEase(Ease.Unset).SetUpdate(true))
                        .Append(rectTransform.DOAnchorPosX(500, 0.33f).SetEase(Ease.Unset).SetUpdate(true))
                        .Pause();

            lobbyManager
                .ObserveEveryValueChanged(l => l.IsSelectedStage.Value)
                .Subscribe(selected =>
                {
                    if (selected)
                    {
                        closeSeq.Complete();
                        openSeq.Restart(true);
                    }
                    else
                    {
                        openSeq.Complete();
                        closeSeq.Restart(true);
                    }
                });
        }
    }
}