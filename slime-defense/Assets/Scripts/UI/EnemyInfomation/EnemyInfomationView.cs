using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.LobbyScene
{
    public class EnemyInfomationView : MonoBehaviour
    {
        //service
        private EnemyInfomationWindow enemyInfomationWindow => ServiceProvider.Get<EnemyInfomationWindow>();
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private Image profile;
        [SerializeField] private TextMeshProUGUI enemyName;
        [SerializeField] private TextMeshProUGUI explain;

        private void Start()
        {
            enemyInfomationWindow
                .ObserveEveryValueChanged(w => w.CurrentSelect)
                .Subscribe(x =>
                {
                    var key = enemyInfomationWindow.CurrentSelect.Key;
                    var data = dataContext.enemyDatas[key];

                    profile.sprite = resourceLoader.enemyIcons[key];
                    enemyName.text = data.name;
                    explain.text = data.explain;
                });
        }
    }
}