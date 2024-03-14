using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        private string current;

        private void Awake()
        {
            ServiceProvider.Register(this, true);

            SceneManager.activeSceneChanged += (_, c) =>
            {
                Debug.Log(c.name);
                Debug.Log(current);
                if(c.name != current)
                {
                    stack.Clear();
                    current = c.name;
                }
            };
        }

        public void LoadNewScene(string sceneName)
        {
            var data = new SceneStackData() { sceneName = SceneManager.GetActiveScene().name, datas = GetCurrentActiveData() };
            stack.Push(data);
            screenFade
                .Fade()
                .UnloadScene(async (_) => DisableAllCurrentGameObject())
                .LoadScene(async () =>
                {
                    current = sceneName;
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                });
        }

        public void LoadPreviousScene(string failure)
        {
            if (stack.TryPop(out var data))
            {
                screenFade
                    .Fade()
                    .UnloadScene(async s => await SceneManager.UnloadSceneAsync(s))
                    .LoadScene(async () =>
                    {
                        current = data.sceneName;
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
                    .LoadScene(async () =>
                    {
                        current = failure;
                        await SceneManager.LoadSceneAsync(failure);
                    });
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