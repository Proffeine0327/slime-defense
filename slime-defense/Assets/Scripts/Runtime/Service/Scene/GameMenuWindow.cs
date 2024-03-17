using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Services
{
    public class GameMenuWindow : MonoBehaviour
    {
        //service
        private SettingWindow settingwindow => ServiceProvider.Get<SettingWindow>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        [SerializeField] private Image bg;
        [SerializeField] private Button returnButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Graphic[] fades;

        private List<Vector2> graphicStartPos = new();

        private void Awake()
        {
            ServiceProvider.Register(this);
            returnButton.onClick.AddListener(() =>
            {
                Hide();
                Time.timeScale = 1;
            });
            settingButton.onClick.AddListener(() =>
            {
                settingwindow.Display(() => Display(true));
                Hide();
            });
            exitButton.onClick.AddListener(() =>
            {
                gameManager.TrySaveGame();
                gameManager.ExitGame();
                Hide();
            });
            foreach (var g in fades)
                graphicStartPos.Add(g.rectTransform.anchoredPosition);
        }

        public void Display(bool immediate = false)
        {
            Time.timeScale = 0;
            bg.gameObject.SetActive(true);
            if (immediate)
            {
                bg.color = new(0, 0, 0, 0.5f);

                var count = 0;
                foreach(var g in fades)
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
                    .SetUpdate(true)
                    .Append(bg.DOColor(new(0, 0, 0, 0.5f), anim).SetUpdate(true));
                var count = 0;
                foreach (var g in fades)
                {
                    g.rectTransform.anchoredPosition = graphicStartPos[count++] + Vector2.up * 15;
                    g.color = new(1, 1, 1, 0);
                    seq.Append(g.rectTransform.DOAnchorPosY(g.rectTransform.anchoredPosition.y - 15, 0.25f).SetUpdate(true));
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