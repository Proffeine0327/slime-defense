using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Services;
using UniRx;

namespace Game.UI.GameScene
{
    public class RoundStartButton : MonoBehaviour
    {
        //services
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.OnClickAsObservable().Subscribe(_ => gameManager.StartWave());
            gameManager.ObserveEveryValueChanged(g => g.IsWaveStart).Subscribe(b => button.interactable = !b);
        }
    }
}