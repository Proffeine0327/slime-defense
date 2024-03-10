using UniRx;
using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UnityEngine;

namespace Game.UI.LobbyScene
{
    public class UnlockStageInfo : MonoBehaviour
    {
        //services
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private AppeareEnemyIcon enemyIconPrefab;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI explain;
        [SerializeField] private RectTransform appeareEnemyIconGroup;
        [SerializeField] private GameObject infinityLock;

        private StageData StageData => dataContext.stageDatas[lobbyManager.Stage.Value - 1];

        private void Start()
        {
            lobbyManager.IsSelectedStage
                .Where(b => b == true)
                .Subscribe(b =>
                {
                    if (!dataContext.userData.unlockStages[lobbyManager.Stage.Value - 1])
                    {
                        gameObject.SetActive(false);
                        return;
                    }

                    gameObject.SetActive(true);
                    title.text = StageData.name;
                    explain.text = StageData.explain;
                    foreach(Transform t in appeareEnemyIconGroup)
                        Destroy(t.gameObject);
                    foreach(var key in StageData.AllAppeareEnemies)
                        Instantiate(enemyIconPrefab, appeareEnemyIconGroup).Set(key);
                    infinityLock.SetActive(!dataContext.userData.unlockInfModes[lobbyManager.Stage.Value]);
                });
        }
    }
}
