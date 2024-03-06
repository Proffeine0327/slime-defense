using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Services;

namespace Game.UI
{
    public class RoundStartButton : MonoBehaviour
    {
        //services
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => gameManager.StartWave());
        }
    }
}