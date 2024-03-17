using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Services
{
    public class AnnounceWindow : MonoBehaviour
    {
        public class Option
        {
            public string title;
            public string explain;
            public Action onSubmit;
            public Action onCancle;
        }

        [SerializeField] private Image bg;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI explain;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancleButton;
        [SerializeField] private Graphic[] fades;

        private List<Vector2> graphicStartPos = new();
        private Option option;

        private void Awake()
        {
            ServiceProvider.Register(this, true);
            foreach (var g in fades)
                graphicStartPos.Add(g.rectTransform.anchoredPosition);
            submitButton.onClick.AddListener(() =>
            {
                option?.onSubmit?.Invoke();
                Hide();
            });
            cancleButton.onClick.AddListener(() =>
            {
                option?.onCancle?.Invoke();
                Hide();
            });
        }

        public void Display(Option option = null, bool immediate = false)
        {
            this.option = option;
            title.text = option?.title;
            explain.text = option?.explain;
            
            bg.gameObject.SetActive(true);
            if (immediate)
            {
                bg.color = new(0, 0, 0, 0.5f);

                var count = 0;
                foreach (var g in fades)
                {
                    g.rectTransform.anchoredPosition = graphicStartPos[count++];
                    g.color = Color.white;
                }
            }
            else
            {
                bg.color = default;
                var anim = 0.33f;
                var seq = DOTween
                    .Sequence()
                    .Append(bg.DOColor(new(0, 0, 0, 0.5f), anim).SetUpdate(true));
                var count = 0;
                foreach (var g in fades)
                {
                    g.rectTransform.anchoredPosition = graphicStartPos[count++] + Vector2.up * 5;
                    g.color = new(1, 1, 1, 0);
                    seq.Append(g.rectTransform.DOAnchorPosY(g.rectTransform.anchoredPosition.y - 5, 0.25f).SetUpdate(true));
                    seq.Join(g.DOColor(Color.white, 0.25f).SetUpdate(true));
                }
                seq.Play();
            }
        }

        public void Hide()
        {
            bg.gameObject.SetActive(false);
        }
    }
}