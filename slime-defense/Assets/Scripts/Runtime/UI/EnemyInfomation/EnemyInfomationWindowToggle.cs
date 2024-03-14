using Game.Services;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

namespace Game.UI
{
    public class EnemyInfomationWindowToggle : MonoBehaviour
    {
        //service
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        //member
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;

        public string Key { get; private set; }

        public void Initialize(string key, EnemyInfomationWindow window)
        {
            Key = key;

            icon.sprite = resourceLoader.enemyIcons[key];
            text.text = dataContext.enemyDatas[key].name;

            GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ => window.Select(this));
        }
    }
}