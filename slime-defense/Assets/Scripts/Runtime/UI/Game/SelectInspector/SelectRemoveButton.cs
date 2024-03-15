using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GameScene
{
    public class SelectRemoveButton : MonoBehaviour
    {
        //service
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        [SerializeField] private TextMeshProUGUI text;
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button
                .OnClickAsObservable()
                .Subscribe(_ => selectManager.Remove());
        }

        private void Update()
        {
            if(selectManager.CurrentSelect == null) return;

            text.text = selectManager.CurrentSelect.RemoveExplain;
            button.interactable = selectManager.CurrentSelect.IsRemovable && !gameManager.IsWaveStart;
        }
    }
}