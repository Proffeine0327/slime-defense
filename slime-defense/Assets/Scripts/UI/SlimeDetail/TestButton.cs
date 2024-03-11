using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.SlimeDetailScene
{
    public class TestButton : MonoBehaviour
    {
        //service
        private SceneNavigation sceneNavigation => ServiceProvider.Get<SceneNavigation>();

        private void Start()
        {
            GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ => sceneNavigation.LoadPreviousScene("development"));
        }
    }
}