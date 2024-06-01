using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Game.UI.LobbyScene
{
    public class StageEnterButtons : MonoBehaviour
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();
        private ScreenFade screenFade => ServiceProvider.Get<ScreenFade>();
        private AnnounceWindow announceWindow => ServiceProvider.Get<AnnounceWindow>();

        [SerializeField] private Button normal;
        [SerializeField] private Button infinity;

        private void Start()
        {
            normal
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (dataContext.userData.saveData == null)
                    {
                        dataContext.userData.CreateNewSaveData(lobbyManager.Stage.Value, false);
                        screenFade
                            .Fade()
                            .LoadScene(async () => await SceneManager.LoadSceneAsync($"Stage{lobbyManager.Stage.Value}"));
                    }
                    else
                    {
                        var option = new AnnounceWindow.Option();
                        option.title = "경고";
                        option.explain = "현재 저장된 게임이 있습니다.\n 정말로 새 게임을 플레이 하시겠습니까?";
                        option.onSubmit = () =>
                        {
                            dataContext.userData.CreateNewSaveData(lobbyManager.Stage.Value, false);
                            screenFade
                                .Fade()
                                .LoadScene(async () => await SceneManager.LoadSceneAsync($"Stage{lobbyManager.Stage.Value}"));
                        };
                        announceWindow.Display(option);
                    }
                });
            infinity
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (dataContext.userData.saveData == null)
                    {
                        dataContext.userData.CreateNewSaveData(lobbyManager.Stage.Value, true);
                        screenFade
                            .Fade()
                            .LoadScene(async () => await SceneManager.LoadSceneAsync($"Stage{lobbyManager.Stage.Value}"));
                    }
                    else
                    {
                        var option = new AnnounceWindow.Option();
                        option.title = "경고";
                        option.explain = "현재 저장된 게임이 있습니다.\n 정말로 새 게임을 플레이 하시겠습니까?";
                        option.onSubmit = () =>
                        {
                            dataContext.userData.CreateNewSaveData(lobbyManager.Stage.Value, true);
                            screenFade
                                .Fade()
                                .LoadScene(async () => await SceneManager.LoadSceneAsync($"Stage{lobbyManager.Stage.Value}"));
                        };
                        announceWindow.Display(option);
                    }
                });
        }
    }
}