using System;
using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    //service
    private GameManager gameManager => ServiceProvider.Get<GameManager>();

    private void Start()
    {
        GetComponent<Button>()
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                gameManager.TrySaveGame();
            });
    }
}