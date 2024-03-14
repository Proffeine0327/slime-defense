using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GameScene
{
    public class LifeDisplayer : MonoBehaviour
    {
        [SerializeField] private float size;

        public ReactiveProperty<bool> IsDisplay = new();

        private RectTransform rectTransform => transform as RectTransform;

        private void Awake()
        {
            IsDisplay
                .Subscribe(b =>
                {
                    DOTween.Kill(transform);
                    rectTransform
                        .DOSizeDelta(b ? Vector2.one * size : Vector2.zero, 0.5f)
                        .SetEase(b ? Ease.OutQuart : Ease.InQuart);
                });
        }
    }
}