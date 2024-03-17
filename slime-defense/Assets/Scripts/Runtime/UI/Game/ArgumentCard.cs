using System;
using Game.GameScene;
using Game.Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.GameScene
{
    public class ArgumentCard : MonoBehaviour, IPointerClickHandler
    {
        //service
        private ArgumentManager argumentManager => ServiceProvider.Get<ArgumentManager>();

        [SerializeField] private int index;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI explain;

        public void Initialize(ArgumentBase argument)
        {
            icon.sprite = argument.Icon ?? icon.sprite;
            title.text = argument.Title;
            explain.text = argument.Explain;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            argumentManager.SelectArgument(index);
        }
    }
}