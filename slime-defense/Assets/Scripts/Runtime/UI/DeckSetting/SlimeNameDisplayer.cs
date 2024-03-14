using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;

namespace Game.UI.DeckSettingScene
{
    public class SlimeNameDisplayer : MonoBehaviour
    {
        //services
        private DeckSettingManager deckSettingManager => ServiceProvider.Get<DeckSettingManager>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        private TextMeshProUGUI text;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();

            deckSettingManager.CurrentSelect
                .Subscribe(s => text.text = s ? "" : dataContext.slimeDatas[s.Key].name);
        }
    }
}