using System.Collections.Generic;
using System.Linq;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;
using Game.GameScene;
using System.Reflection;
using System;

namespace Game.Services
{
    public class ArgumentManager : MonoBehaviour
    {
        //inject
        private DataContext dataManager => ServiceProvider.Get<DataContext>();
        private SlimeManager unitManager => ServiceProvider.Get<SlimeManager>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        [SerializeField] private Image bg;
        // [SerializeField] private ArgumentCardUI[] selectCards;
        [SerializeField] private PopupTrigger argumentIconPrefab;
        [SerializeField] private Explain explain;
        [SerializeField] private RectTransform argumentIconGroup;

        private List<ArgumentBase> unuse = new();
        private List<ArgumentBase> use = new();
        private ArgumentBase[] display;

        public List<ArgumentBase> Use => use;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

        private void Start()
        {
            // display = new ArgumentBase[selectCards.Length];
            var gameData = dataManager.userData.saveData;

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ArgumentBase))).ToArray();
            var instances = types.Select(t => Activator.CreateInstance(t) as ArgumentBase).ToArray();
            foreach (var argument in instances)
            {
                if (gameData.arguments != null && gameData.arguments.Contains(argument.Name))
                    use.Add(argument);
                else
                    unuse.Add(argument);
            }

            // UpdateArgument();
            unitManager.OnSlimeUpdate += () =>
            {
                foreach (var item in use)
                    item.OnSlimeUpdate(unitManager.Slimes.ToArray());
            };
        }

        // public void DisplayArgument()
        // {
        //     var list = unuse.ToList();
        //     for (int i = 0; i < selectCards.Length; i++)
        //     {
        //         var randomIndex = Random.Range(0, list.Count);
        //         display[i] = list[randomIndex];

        //         selectCards[i].Init(list[randomIndex]);
        //         list.RemoveAt(randomIndex);
        //     }
        //     bg.gameObject.SetActive(true);
        // }

        // public void SelectArgument(int index)
        // {
        //     unuse.Remove(display[index]);
        //     use.Add(display[index]);

        //     UpdateArgument();
        //     bg.gameObject.SetActive(false);
        // }

        // private void UpdateArgument()
        // {
        //     while (argumentIconGroup.childCount < use.Count)
        //     {
        //         var argument = use[argumentIconGroup.childCount];
        //         var popupTrigger = Instantiate(argumentIconPrefab, argumentIconGroup);
        //         popupTrigger.Image.sprite = argument.icon;
        //         popupTrigger.SetPopup(explain);
        //         popupTrigger.OnChangeState += isOpen =>
        //         {
        //             if (!isOpen) return;
        //             explain.Init(argument.icon, argument.title, argument.explain);
        //             explain.Display();
        //         };
        //     }
        // }
    }
}