using System.Collections.Generic;
using System.Linq;
using Game.GameScene;
using Game.UI;
using Game.UI.GameScene;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;
using UniRx;
using DG.Tweening;

namespace Game.Services
{
    public class ArgumentManager : MonoBehaviour
    {
        //services
        private DataContext dataManager => ServiceProvider.Get<DataContext>();
        private SlimeManager unitManager => ServiceProvider.Get<SlimeManager>();

        [SerializeField] private Image bg;
        [SerializeField] private ArgumentCard[] selectCards;
        [SerializeField] private PopupTrigger argumentIconPrefab;
        [SerializeField] private RectTransform argumentIconGroup;
        [SerializeField] private Explain explain;

        private List<ArgumentBase> unuse = new();
        private ReactiveCollection<ArgumentBase> use = new();
        private ArgumentBase[] display = new ArgumentBase[3];

        public IReadOnlyReactiveCollection<ArgumentBase> Use => use;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

        private void Start()
        {
            var gameData = dataManager.userData.saveData;

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ArgumentBase)) && t != typeof(ArgumentBase)).ToArray();
            var instances = types.Select(t => Activator.CreateInstance(t) as ArgumentBase).ToArray();

            foreach (var argument in instances)
            {
                if (gameData.arguments != null && gameData.arguments.Contains(argument.Title))
                    use.Add(argument);
                else
                    unuse.Add(argument);
            }
            
            use
                .ObserveAdd()
                .Subscribe(_ =>
                {
                    var argument = use[argumentIconGroup.childCount];
                    var popupTrigger = Instantiate(argumentIconPrefab, argumentIconGroup);
                    popupTrigger.Sprite = argument.Icon;
                    popupTrigger.SetPopup(explain);
                    popupTrigger.OnChangeState += isOpen =>
                    {
                        if (!isOpen) return;
                        explain.Display(argument.Icon, argument.Title, argument.Explain);
                    };
                });

            unitManager.OnSlimeUpdate += () =>
            {
                foreach (var item in use)
                    item.OnSlimeUpdate();
            };
        }

        public void DisplayArgument()
        {
            var list = unuse.ToList();
            for (int i = 0; i < selectCards.Length; i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, list.Count);
                display[i] = list[randomIndex];

                selectCards[i].Initialize(list[randomIndex]);
                list.RemoveAt(randomIndex);
            }
            bg.gameObject.SetActive(true);
            bg.color = default;
            bg.DOColor(new Color(0, 0, 0, 0.5f), 0.25f);
            foreach(var card in selectCards)
            {
                card.transform.localScale = new Vector3(0.01f, 1, 1);
                card.transform.DOScaleX(1, 0.25f);
            }
        }

        public void SelectArgument(int index)
        {
            unuse.Remove(display[index]);
            use.Add(display[index]);
            bg.gameObject.SetActive(false);
            display[index].OnAdd();
        }
    }
}