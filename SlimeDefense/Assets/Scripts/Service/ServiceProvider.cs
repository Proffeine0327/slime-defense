using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceProvider : MonoBehaviour
{
    private static ServiceProvider _instance;

    public static void Register<T>(T instance, bool isGlobal = false) where T : MonoBehaviour
    {
        if (_instance == null)
        {
            _instance = new GameObject("[ServiceProvider]").AddComponent<ServiceProvider>();
            DontDestroyOnLoad(_instance.gameObject);
            SceneManager.sceneUnloaded += _ =>
            {
                _instance.sceneContext.Clear();
                Debug.Log("Clear");
            };
        }

        if (isGlobal)
        {
            if (!_instance.globalContext.ContainsKey(typeof(T).FullName))
            {
                _instance.globalContext.Add(typeof(T).FullName, instance);
                DontDestroyOnLoad(instance.gameObject);
            }
            else
                Debug.LogWarning($"{typeof(T).FullName} type already exist in global context.");
        }
        else
        {
            if (!_instance.sceneContext.ContainsKey(typeof(T).FullName))
                _instance.sceneContext.Add(typeof(T).FullName, instance);
            else
                Debug.LogWarning($"{typeof(T).FullName} type already exist in scene context.");
        }
    }

    public static T Get<T>() where T : MonoBehaviour
    {
        T inst = _instance.sceneContext[typeof(T).FullName] as T;
        if (inst) return inst;
        return _instance.globalContext[typeof(T).FullName] as T;
    }

    private readonly Dictionary<string, MonoBehaviour> globalContext = new();
    private readonly Dictionary<string, MonoBehaviour> sceneContext = new();
}
