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
                .OnSceneUnload(() => DisableAllCurrentGameObject())
                .LoadScene(() => SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive), sceneName);
        }

        public void LoadPreviousScene(string failure)
        {
            if (stack.TryPop(out var data))
            {
                screenFade
                    .Fade()
                    .UnloadScene(s =>SceneManager.UnloadSceneAsync(s))
                    .OnSceneLoad(() =>
                    {
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
                    .LoadScene(() => SceneManager.LoadSceneAsync(failure), failure);
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