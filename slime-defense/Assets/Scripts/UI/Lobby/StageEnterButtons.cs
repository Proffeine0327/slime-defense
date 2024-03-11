using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI.LobbyScene
{
    public class StageEnterButtons : MonoBehaviour
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();
        private ScreenFade screenFade => ServiceProvider.Get<ScreenFade>();

        [SerializeField] private Button normal;
        [SerializeField] private Button infinity;

        private void Start()
        {
            normal
                .OnClickAsObservable()
                .Subscribe(_ => 
                {
                    dataContext.userData.CreateNewSaveData(lobbyManager.Stage.Value, false);
                    screenFade
                        .Fade()
                        .SceneLoad(() => SceneManager.LoadScene($"Stage{lobbyManager.Stage.Value}"));
                });
            infinity
                .OnClickAsObservable()
                .Subscribe(_ => 
                {
                    dataContext.userData.CreateNewSaveData(lobbyManager.Stage.Value, true);
                    screenFade
                        .Fade()
                        .SceneLoad(() => SceneManager.LoadScene($"Stage{lobbyManager.Stage.Value}"));
                });
        }
    }
}