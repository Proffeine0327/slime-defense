using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Game.Services;
using Game.GameScene;

namespace Game.UI
{
    public class SelectStatValueDisplayer : MonoBehaviour
    {
        //services
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();

        [SerializeField] private Stats.Key key;

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
                text.text = target.DisplayStat.GetStat(key).ToString("#,##0.##");
        }
    }
}