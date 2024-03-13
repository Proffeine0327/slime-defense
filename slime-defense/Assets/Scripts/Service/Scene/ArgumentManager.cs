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

namespace Game.Services
{
    public class ArgumentManager : MonoBehaviour
    {
        //inject
        private DataContext dataManager => ServiceProvider.Get<DataContext>();
        private SlimeManager unitManager => ServiceProvider.Get<SlimeManager>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        [SerializeField] private Image bg;
        [SerializeField] private ArgumentCard[] selectCards;
        [SerializeField] private PopupTrigger argumentIconPrefab;
        [SerializeField] private RectTransform argumentIconGroup;
        [SerializeField] private Explain explain;

        private List<ArgumentBase> unuse = new();
        private ReactiveCollection<ArgumentBase> use = new();
        private ArgumentBase[] display;

        public IReadOnlyReactiveCollection<ArgumentBase> Use => use;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

        private void Start()
        {
            var gameData = dataManager.userData.saveData;

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ArgumentBase))).ToArray();
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
                    popupTrigger.Image.sprite = argument.Icon;
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
                    item.OnSlimeUpdate(unitManager.Slimes.ToArray());
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
        }

        public void SelectArgument(int index)
        {
            unuse.Remove(display[index]);
            use.Add(display[index]);
            bg.gameObject.SetActive(false);
        }
    }
}