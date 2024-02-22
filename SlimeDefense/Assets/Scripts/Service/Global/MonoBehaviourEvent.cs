using System;
using UnityEngine;

public class MonoBehaviourEvent : MonoBehaviour
{
    public event Action OnUpdate;

    private void Awake()
    {
        ServiceProvider.Register(this, true);
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }
}