using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class ArgumentManager : MonoBehaviour, ISaveLoad, IInitialize
    {
        //services
        private DataContext dataManager => ServiceProvider.Get<DataContext>();
        private SlimeManager unitManager => ServiceProvider.Get<SlimeManager>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        [SerializeField] private Image bg;
        [SerializeField] private ArgumentCard[] selectCards;
        [SerializeField] private PopupTrigger argumentIconPrefab;
        [SerializeField] private RectTransform argumentIconGroup;
        [SerializeField] private Explain explain;

        private bool isSelected;
        private Dictionary<string, ArgumentBase> arguments = new();
        private HashSet<string> unuse = new();
        private ReactiveCollection<string> use = new();
        private string[] display = new string[3];

        public IReadOnlyList<string> Use => use;
        public bool IsSelected => isSelected;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

        public void Initialize()
        {
            var gameData = gameManager.SaveData;

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ArgumentBase)) && t != typeof(ArgumentBase)).ToArray();
            var instances = types.Select(t => Activator.CreateInstance(t) as ArgumentBase).ToArray();

            foreach (var argument in instances)
            {
                var typeName = argument.GetType().FullName;
                arguments.Add(typeName, argument);
                unuse.Add(typeName);
            }

            use
                .ObserveAdd()
                .Subscribe(_ =>
                {
                    var argument = arguments[use[^1]];
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
                    arguments[item].OnSlimeUpdate();
            };
        }

        public void DisplayArgument()
        {
            isSelected = false;
            var list = unuse.ToList();
            for (int i = 0; i < selectCards.Length; i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, list.Count);
                display[i] = list[randomIndex];

                selectCards[i].Initialize(arguments[list[randomIndex]]);
                list.RemoveAt(randomIndex);
            }
            bg.gameObject.SetActive(true);
            bg.color = default;
            bg.DOColor(new Color(0, 0, 0, 0.5f), 0.25f);
            foreach (var card in selectCards)
            {
                card.transform.localScale = new Vector3(0.01f, 1, 1);
                card.transform.DOScaleX(1, 0.25f);
            }
        }

        public void SelectArgument(int index)
        {
            isSelected = true;
            unuse.Remove(display[index]);
            use.Add(display[index]);
            bg.gameObject.SetActive(false);
            arguments[display[index]].OnAdd();
        }

        public string Save()
        {
            var data = new StringListWrapper();
            foreach (var u in use)
                data.datas.Add($"{u}\'{arguments[u].Save()}");
            return JsonUtility.ToJson(data);
        }

        public void Load(string json)
        {
            Initialize();
            var data = JsonUtility.FromJson<StringListWrapper>(json);
            foreach (var d in data.datas)
            {
                var key = d[0..d.IndexOf('\'')];
                var argumentJson = d[(d.IndexOf('\'') + 1)..];

                unuse.Remove(key);
                use.Add(key);
                arguments[key].Load(argumentJson);
            }
        }
    }
}