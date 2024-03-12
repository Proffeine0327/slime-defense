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
            public Action onSceneUnload { get; private set; }
            public Func<AsyncOperation> loadScene { get; private set; }
            public string loadSceneName { get; private set; }
            public Func<string, AsyncOperation> unloadScene { get; private set; }
            public LoadSceneMode mode { get; private set; }

            public ScreenFadeSetting OnSceneLoad(Action onSceneLoad)
            {
                this.onSceneLoad += onSceneLoad;
                return this;
            }

            public ScreenFadeSetting OnSceneUnload(Action onSceneUnload)
            {
                this.onSceneUnload += onSceneUnload;
                return this;
            }

            public ScreenFadeSetting LoadScene(Func<AsyncOperation> loadScene, string sceneName)
            {
                this.loadScene = loadScene;
                this.loadSceneName = sceneName;
                return this;
            }

            public ScreenFadeSetting UnloadScene(Func<string, AsyncOperation> unloadScene)
            {
                this.unloadScene = unloadScene;
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

            if (setting.unloadScene != null)
            {
                var operation = setting.unloadScene.Invoke(SceneManager.GetActiveScene().name);
                while(!operation.isDone) yield return waitReal;
            }
            setting.onSceneUnload?.Invoke();
            
            if (setting.loadScene != null)
            {
                var operation = setting.loadScene.Invoke();
                while(!operation.isDone) yield return waitReal;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(setting.loadSceneName));
            }
            setting.onSceneLoad?.Invoke();
            yield return new WaitForSecondsRealtime(0.5f);

            blinder.DOColor(default, 0.75f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.75f);
            blinder.gameObject.SetActive(false);
        }
    }
}