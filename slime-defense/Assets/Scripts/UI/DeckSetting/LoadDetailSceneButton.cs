using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.DeckSettingScene
{
    public class LoadDetailSceneButton : MonoBehaviour
    {
        //services
        private SceneNavigation sceneNavigation => ServiceProvider.Get<SceneNavigation>();
        private DeckSettingManager deckSettingManager => ServiceProvider.Get<DeckSettingManager>();
        private CameraManager cameraManager => ServiceProvider.Get<CameraManager>();
        
        private Button button;

        private RectTransform rectTransform => transform as RectTransform;

        private void Start()
        {
            button = GetComponentInChildren<Button>();

            button
                .OnClickAsObservable()
                .Subscribe(_ => sceneNavigation.LoadNewScene("SlimeDetail"));
        }

        private void Update()
        {
            var current = deckSettingManager.CurrentSelect.Value;
            if(!current)
            {
                button.gameObject.SetActive(false);
                return;
            }

            button.gameObject.SetActive(true);
            rectTransform.anchoredPosition = cameraManager.GetScreenPosition(current.transform);
        }
    }
}