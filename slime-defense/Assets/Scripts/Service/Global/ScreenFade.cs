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
            public Action onSceneLoad { get; private set; }
            public Action onActiveSceneUnload { get; private set; }
            public bool immediately;

            public ScreenFadeSetting OnSceneLoad(Action onSceneLoad)
            {
                this.onSceneLoad += onSceneLoad;
                return this;
            }

            public ScreenFadeSetting OnActiveSceneUnload(Action onActiveSceneUnload)
            {
                this.onActiveSceneUnload += onActiveSceneUnload;
                return this;
            }

            public ScreenFadeSetting SetImmediately(bool immediately)
            {
                this.immediately = immediately;
                return this;
            }
        }

        [SerializeField] private Image blinder;
        [SerializeField] private float leastLoadTime;

        private void Awake()
        {
            ServiceProvider.Register(this, true);
        }

        public ScreenFadeSetting LoadScene(string sceneName)
        {
            var setting = new ScreenFadeSetting();
            StartCoroutine(LoadRoutine(sceneName, setting));
            return setting;
        }

        private IEnumerator LoadRoutine(string sceneName, ScreenFadeSetting setting)
        {
            var waitReal = new WaitForSecondsRealtime(0f);
            yield return waitReal;

            blinder.gameObject.SetActive(true);
            blinder.DOColor(Color.black, 0.75f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(1.5f);

            if (!setting.immediately)
            {
                SceneManager.LoadScene("Loading");
                yield return waitReal;
                setting.onActiveSceneUnload?.Invoke();

                blinder.DOColor(default, 1f).SetUpdate(true);
                yield return new WaitForSecondsRealtime(1f);
                blinder.gameObject.SetActive(false);

                var load = SceneManager.LoadSceneAsync(sceneName);
                load.allowSceneActivation = false;
                for (float t = 0; t < leastLoadTime && !load.isDone; t += Time.unscaledDeltaTime)
                    yield return waitReal;

                blinder.gameObject.SetActive(true);
                blinder.DOColor(Color.black, 0.75f).SetUpdate(true);
                yield return new WaitForSecondsRealtime(1.5f);

                load.allowSceneActivation = true;
                yield return waitReal;
                setting.onActiveSceneUnload?.Invoke();
            }
            else
            {
                var load = SceneManager.LoadSceneAsync(sceneName);
                load.allowSceneActivation = false;
                for (float t = 0; t < leastLoadTime && !load.isDone; t += Time.unscaledDeltaTime)
                    yield return waitReal;
                load.allowSceneActivation = true;
                setting.onActiveSceneUnload?.Invoke();
            }
            setting.onSceneLoad?.Invoke();
            yield return waitReal;

            blinder.DOColor(default, 0.75f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(1.5f);
            blinder.gameObject.SetActive(false);
        }
    }
}