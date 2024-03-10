using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.LoadScene
{
    public class PassLoadingScene : MonoBehaviour
    {
        //service
        private TaskWaiter taskWaiter => ServiceProvider.Get<TaskWaiter>();
        private ScreenFade screenFade => ServiceProvider.Get<ScreenFade>();

        private void Start()
        {
            taskWaiter
                .ObserveEveryValueChanged(w => w.IsEndLoad)
                .Where(b => b == true)
                .Subscribe(b => screenFade.LoadScene("Lobby").SetImmediately(true));
        }
    }
}