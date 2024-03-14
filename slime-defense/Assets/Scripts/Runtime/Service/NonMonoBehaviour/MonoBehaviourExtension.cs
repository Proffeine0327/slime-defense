using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void Invoke(this MonoBehaviour mb, Action action, float t, bool isUpdate = false)
    {
        mb.StartCoroutine(InternerInvoke(action, t, isUpdate));
    }

    private static IEnumerator InternerInvoke(Action action, float t, bool isUpdate)
    {
        if(isUpdate)
            yield return new WaitForSecondsRealtime(t);
        else
            yield return new WaitForSeconds(t);
        action?.Invoke();
    }
}