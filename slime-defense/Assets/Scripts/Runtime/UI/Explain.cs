using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class Explain : Popup
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI explain;

        public void Display(Sprite icon, string title, string explain)
        {
            this.icon.sprite = icon;
            this.title.text = title;
            this.explain.text = explain;
            Display();
        }
    }
}