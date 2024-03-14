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

        [SerializeField] private TextMeshProUGUI text;
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button
                .OnClickAsObservable()
                .Subscribe(_ => selectManager.Remove());
            selectManager.OnSelect += select =>
            {
                if (select != null)
                {
                    text.text = select.RemoveExplain;
                    button.interactable = select.IsRemovable;
                }
            };
        }
    }
}