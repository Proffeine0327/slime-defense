using Game.GameScene;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;

namespace Game.UI.LobbyScene
{
    public class EnemyInfomationStat : MonoBehaviour
    {
        //service
        private EnemyInfomationWindow window => ServiceProvider.Get<EnemyInfomationWindow>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private Stats.Key target;
        [SerializeField] private TextMeshProUGUI baseStat;
        [SerializeField] private TextMeshProUGUI lvUpStat;

        private void Start()
        {
            window
                .ObserveEveryValueChanged(x => x.CurrentSelect)
                .Subscribe(s =>
                {
                    baseStat.text = dataContext.enemyDatas[s.Key].@base.GetStat(target).ToString("#,##0.##");

                    var percent = dataContext.enemyDatas[s.Key].percentage.GetStat(target);
                    var add = dataContext.enemyDatas[s.Key].add.GetStat(target);
                    lvUpStat.text = $"{percent:#,##0.#}%+{add:#,##0.#}";
                });
        }
    }
}