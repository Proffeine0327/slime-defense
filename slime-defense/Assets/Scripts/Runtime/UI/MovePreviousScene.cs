using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MovePreviousScene : MonoBehaviour
    {
        //service
        private SceneNavigation sceneNavigation => ServiceProvider.Get<SceneNavigation>();

        private void Start()
        {
            GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ => sceneNavigation.LoadPreviousScene("Lobby"));
        }
    }
}