using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

namespace Game.Services
{
    public class ScreenFade : MonoBehaviour
    {
        public class ScreenFadeSetting
        {
            public Action sceneLoadRoutine { get; private set; }
            public Action<string> sceneUnloadRoutine { get; private set; }
            public LoadSceneMode mode { get; private set; }

            public ScreenFadeSetting SceneLoad(Action sceneLoadRoutine)
            {
                this.sceneLoadRoutine += sceneLoadRoutine;
                return this;
            }

            public ScreenFadeSetting SceneUnload(Action<string> sceneUnloadRoutine)
            {
                this.sceneUnloadRoutine += sceneUnloadRoutine;
                return this;
            }

            public ScreenFadeSetting SetLoadSceneMode(LoadSceneMode mode)
            {
                this.mode = mode;
                return this;
            }
        }

        [SerializeField] private Image blinder;

        private void Awake()
        {
            ServiceProvider.Register(this, true);
        }

        public ScreenFadeSetting Fade()
        {
            var setting = new ScreenFadeSetting();
            StartCoroutine(LoadRoutine(setting));
            return setting;
        }

        private IEnumerator LoadRoutine(ScreenFadeSetting setting)
        {
            var waitReal = new WaitForSecondsRealtime(0f);
            yield return waitReal;

            blinder.gameObject.SetActive(true);
            blinder.DOColor(Color.black, 0.75f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(1.5f);

            setting.sceneUnloadRoutine?.Invoke(SceneManager.GetActiveScene().name);
            setting.sceneLoadRoutine?.Invoke();
            yield return new WaitForSecondsRealtime(0.5f);

            blinder.DOColor(default, 0.75f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.75f);
            blinder.gameObject.SetActive(false);
        }
    }
}