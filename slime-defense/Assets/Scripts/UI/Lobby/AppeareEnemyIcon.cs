using System.Collections.Generic;
using Game.Services;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Game.UI.LobbyScene
{
    public class AppeareEnemyIcon : MonoBehaviour
    {
        //services
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();
        private EnemyInfomationWindow enemyInfomationWindow => ServiceProvider.Get<EnemyInfomationWindow>();

        private string key;
        private Image image;
        private Button button;

        private Image Image
        {
            get
            {
                if (!image)
                    image = GetComponent<Image>();
                return image;
            }
        }

        private Button Button
        {
            get
            {
                if (!button)
                    button = GetComponent<Button>();
                return button;
            }
        }

        private void Start()
        {
            Button
                .OnClickAsObservable()
                .Subscribe(_ => enemyInfomationWindow.Open(key));
        }

        public void Set(string key)
        {
            this.key = key;
            Image.sprite = resourceLoader.enemyIcons.GetValueOrDefault(key);
        }
    }
}