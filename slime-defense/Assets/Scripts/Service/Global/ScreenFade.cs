using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;

namespace Game.Services
{
    public class ScreenFade : MonoBehaviour
    {
        public class ScreenFadeSetting
        {
            public Func<UniTask> loadScene { get; private set; }
            public Func<string, UniTask> unloadScene { get; private set; }

            public ScreenFadeSetting LoadScene(Func<UniTask> loadScene)
            {
                this.loadScene = loadScene;
                return this;
            }

            public ScreenFadeSetting UnloadScene(Func<string, UniTask> unloadScene)
            {
                this.unloadScene = unloadScene;
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
            LoadRoutine(setting).Forget();
            return setting;
        }

        private async UniTaskVoid LoadRoutine(ScreenFadeSetting setting)
        {
            blinder.gameObject.SetActive(true);
            blinder.DOColor(Color.black, 0.75f).SetUpdate(true);
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

            if (setting.unloadScene != null)
                await setting.unloadScene.Invoke(SceneManager.GetActiveScene().name);
            
            if (setting.loadScene != null)
                await setting.loadScene.Invoke();

            await UniTask.DelayFrame(1);
            
            blinder.DOColor(default, 0.75f).SetUpdate(true);
            await UniTask.Delay(TimeSpan.FromSeconds(0.75f));
            blinder.gameObject.SetActive(false);
        }
    }
}