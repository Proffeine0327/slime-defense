using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UnityEngine;
using UniRx;
using TMPro;

namespace Game.UI.LobbyScene
{
    public class LockStageInfo : MonoBehaviour
    {
        //services
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        private StageData StageData => dataContext.stageDatas[lobbyManager.Stage.Value - 1];

        [SerializeField] private TextMeshProUGUI explain;

        private void Start()
        {
            lobbyManager.IsSelectedStage
                .Where(b => b == true)
                .Subscribe(b =>
                {
                    if (dataContext.userData.unlockStages[lobbyManager.Stage.Value - 1])
                    {
                        gameObject.SetActive(false);
                        return;
                    }

                    gameObject.SetActive(true);
                    explain.text = $"해금: {StageData.unlockMoney}<sprite=\"coin\" name=\"coin\">";
                });
        }
    }
}