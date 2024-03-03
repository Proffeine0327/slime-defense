using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void Invoke(this MonoBehaviour mb, Action action, float t)
    {
        mb.StartCoroutine(InternerInvoke(action, t));
    }

    private static IEnumerator InternerInvoke(Action action, float t)
    {
        yield return new WaitForSeconds(t);
        action?.Invoke();
    }
}