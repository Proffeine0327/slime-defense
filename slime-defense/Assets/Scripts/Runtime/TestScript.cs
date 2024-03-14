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

    // private IEnumerator Start()
    // {
    //     var load = SceneManager.LoadSceneAsync("Lobby");
    //     load.allowSceneActivation = false;
    //     load.
    //     yield return load;
    //     Debug.Log("test");
    // }
}