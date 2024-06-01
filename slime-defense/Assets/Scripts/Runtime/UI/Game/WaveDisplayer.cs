using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;

namespace Game.UI.GameScene
{
    public class WaveDisplayer : MonoBehaviour
    {
        //service
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        private TextMeshProUGUI text;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
            gameManager.SaveData
                .ObserveEveryValueChanged(x => x.wave)
                .Subscribe(x => text.text = gameManager.SaveData.isInfinity ? x.ToString() : $"{x}/{gameManager.MaxWave}");
        }
    }
}