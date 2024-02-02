using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceProvider : MonoBehaviour
{
    private static ServiceProvider _instance;

    public static void Register<T>(T instance) where T : MonoBehaviour
    {
        if(_instance == null)
            _instance = new GameObject("[ServiceProvider]").AddComponent<ServiceProvider>();

        Type t = typeof(T);
        do
        {
            if(!_instance.services.ContainsKey(t.FullName))
                _instance.services.Add(t.FullName, instance);
            else
                Debug.LogWarning($"There were two or more scripts with the same type or base type within scene. \nLater instance was ignored \n\nRegistered : {_instance.services[t.FullName].name}({t.FullName}) \nIgnored : {instance.name}({typeof(T).FullName})\n");
            t = t.BaseType;
        } while(t != typeof(MonoBehaviour));
    }

    public static T Get<T>() where T : MonoBehaviour
        => _instance.services[typeof(T).FullName] as T;

    private readonly Dictionary<string, MonoBehaviour> services = new();
}
