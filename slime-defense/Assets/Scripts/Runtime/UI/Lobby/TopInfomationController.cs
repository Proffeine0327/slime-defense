using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Game.UI.GameScene
{
    public class TopInfomationController : MonoBehaviour
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private LobbyMenuWindow lobbyMenuWindow => ServiceProvider.Get<LobbyMenuWindow>();
        private ScreenFade screenFade => ServiceProvider.Get<ScreenFade>();

        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Button loadSaveButton;
        [SerializeField] private Button settingButton;

        private void Start()
        {
            dataContext.userData
                .ObserveEveryValueChanged(d => d.money)
                .Subscribe(m => moneyText.text = m.ToString("#,##0"));
            dataContext.userData
                .ObserveEveryValueChanged(d => d.hp)
                .Subscribe(m => hpText.text = m.ToString("#,##0"));
            loadSaveButton
                .onClick
                .AddListener(() =>
                {
                    screenFade
                        .Fade()
                        .LoadScene(async () => await SceneManager.LoadSceneAsync($"Stage{dataContext.userData.saveData.stage}"));
                });
            settingButton.onClick.AddListener(() => lobbyMenuWindow.Display());
        }

        private void Update()
        {
            loadSaveButton.interactable = dataContext.userData.saveData != null;
        }
    }
}