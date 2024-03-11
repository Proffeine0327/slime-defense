using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Services
{
    public class SceneNavigation : MonoBehaviour
    {
        public struct SceneStackData
        {
            public string sceneName;
            public Dictionary<GameObject, bool> datas;
        }

        //service
        private ScreenFade screenFade => ServiceProvider.Get<ScreenFade>();

        private Stack<SceneStackData> stack = new();

        private void Awake()
        {
            ServiceProvider.Register(this, true);
        }

        public void LoadNewScene(string sceneName)
        {
            var data = new SceneStackData() { sceneName = SceneManager.GetActiveScene().name, datas = GetCurrentActiveData() };
            stack.Push(data);
            screenFade
                .Fade()
                .SceneUnload(_ => DisableAllCurrentGameObject())
                .SceneLoad(() =>
                {
                    SceneManager
                        .LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                        .completed += _ => SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                });
        }

        public void LoadPreviousScene(string failure)
        {
            if (stack.TryPop(out var data))
            {
                screenFade
                    .Fade()
                    .SceneUnload(s => SceneManager.UnloadSceneAsync(s))
                    .SceneLoad(() =>
                    {
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(data.sceneName));
                        foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
                        {
                            if (data.datas.ContainsKey(obj))
                                obj.SetActive(data.datas[obj]);
                        }
                    });

            }
            else
            {
                screenFade
                    .Fade()
                    .SceneLoad(() => SceneManager.LoadScene(failure));
            }
        }

        private Dictionary<GameObject, bool> GetCurrentActiveData()
        {
            var dic = new Dictionary<GameObject, bool>();
            foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
                dic.Add(obj, obj.activeSelf);
            return dic;
        }

        private void DisableAllCurrentGameObject()
        {
            foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
                obj.SetActive(false);
        }
    }
}