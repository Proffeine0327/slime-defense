using Game.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

namespace Game.Services
{
    public class EnemyInfomationWindow : MonoBehaviour
    {
        //service
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private Image bg;
        [SerializeField] private RectTransform book;
        [SerializeField] private Button exit;
        [SerializeField] private EnemyInfomationWindowToggle togglePrefab;
        [SerializeField] private RectTransform toggleGroup;

        private EnemyInfomationWindowToggle currentSelect;
        private List<EnemyInfomationWindowToggle> toggles = new();

        public EnemyInfomationWindowToggle CurrentSelect => currentSelect;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

        private void Start()
        {
            foreach (var data in dataContext.enemyDatas)
            {
                var toggle = Instantiate(togglePrefab, toggleGroup);
                toggle.Initialize(data.Key, this);
                if (currentSelect == null)
                    currentSelect = toggle;
                toggles.Add(toggle);
            }

            exit
                .OnClickAsObservable()
                .Subscribe(_ => Close());
        }
                
        public void Open(string key)
        {
            foreach(var t in toggles)
            {
                if(t.Key == key)
                {
                    Select(t);
                    break;
                }
            }
            Open();
        }

        public void Open()
        {
            bg.color = new Color(0, 0, 0, 0);
            book.localScale = Vector3.one * 0.01f;

            bg.DOColor(new Color(0, 0, 0, 0.54f), 0.5f).SetUpdate(true);
            book.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

            bg.gameObject.SetActive(true);
            //SoundManager.PlaySound("UI_Blop", 1);
        }

        public void Close()
        {
            bg.color = new Color(0, 0, 0, 0.54f);
            book.localScale = Vector3.one;

            bg.DOColor(new Color(0, 0, 0, 0), 0.5f).SetUpdate(true);
            book.DOScale(0.01f, 0.5f).SetEase(Ease.InCubic).SetUpdate(true);

            this.Invoke(() => bg.gameObject.SetActive(false), 0.5f, true);
            //SoundManager.PlaySound("UI_Blop", 1);
        }

        public void Select(EnemyInfomationWindowToggle select)
            => currentSelect = select;
    }
}