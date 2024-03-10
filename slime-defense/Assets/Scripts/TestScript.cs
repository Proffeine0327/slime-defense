using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    private ReactiveProperty<int> test = new();

    private IEnumerator Start()
    {
        test
            .ThrottleFirst(TimeSpan.FromSeconds(2f))
            .Subscribe(x => Debug.Log($"Subscribe: {x}"));

        yield return new WaitForSeconds(1f);
        test.Value++;
        Debug.Log($"Debug: {test.Value}");
        
        yield return new WaitForSeconds(1f);
        test.Value++;
        Debug.Log($"Debug: {test.Value}");
        
        yield return new WaitForSeconds(1f);
        test.Value++;
        Debug.Log($"Debug: {test.Value}");
    }

    // private void Update()
    // {
    //     Debug.Log($"Update: {test.Value}");
    // }
}