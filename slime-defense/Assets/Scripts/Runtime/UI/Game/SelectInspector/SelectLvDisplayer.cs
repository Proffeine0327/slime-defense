using Game.Services;
using TMPro;
using UnityEngine;
using Game.GameScene;

namespace Game.UI.GameScene
{
    public class SelectLvDisplayer : MonoBehaviour
    {
        //serivce
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        private ISelectable target;
        private TextMeshProUGUI text;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
            selectManager.OnSelect += select => target = select;
        }

        private void Update()
        {
            if (target != null)
            {
                var lvStr = target.Lv == dataContext.gameData.maxLv ? "Max" : target.Lv.ToString();
                text.text = $"Lv. {lvStr}";
            }
        }
    }
}