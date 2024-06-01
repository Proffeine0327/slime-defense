using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;
using UniRx.Triggers;

namespace Game.UI.LobbyScene
{
    public class LockStageInfo : MonoBehaviour
    {
        //services
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private AnnounceWindow announceWindow => ServiceProvider.Get<AnnounceWindow>();

        private StageData StageData => dataContext.stageDatas[lobbyManager.Stage.Value - 1];

        [SerializeField] private GameObject group;
        [SerializeField] private TextMeshProUGUI explain;
        [SerializeField] private Button unlockButton;

        private void Start()
        {
            lobbyManager.IsSelectedStage
                .Where(b => b == true)
                .Subscribe(b =>
                {
                    gameObject.SetActive(true);
                    explain.text = $"�ر�: {StageData.unlockMoney}<sprite=\"coin\" name=\"coin\">";
                });

            this.UpdateAsObservable()
                .Subscribe(_ => group.SetActive(!dataContext.userData.unlockStages[lobbyManager.Stage.Value - 1]));
            
            unlockButton.onClick.AddListener(() =>
            {
                var option = new AnnounceWindow.Option()
                {
                    title = "구매 ?���?",
                    explain = $"��������{lobbyManager.Stage.Value}�� �����Ͻðڽ��ϱ�?\n��� {dataContext.userData.money} �� {dataContext.userData.money - StageData.unlockMoney}",
                    onSubmit = () =>
                    {
                        dataContext.userData.unlockStages[lobbyManager.Stage.Value - 1] = true;
                        dataContext.userData.money -= StageData.unlockMoney;
                    }
                };
                announceWindow.Display(option);
            });
        }
    }
}