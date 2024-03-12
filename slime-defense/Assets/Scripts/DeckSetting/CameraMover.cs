using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UnityEngine;

namespace Game.DeckSettingScene
{
    public class CameraMover : MonoBehaviour
    {
        //services
        private DeckSettingManager deckSettingManager => ServiceProvider.Get<DeckSettingManager>();

        [SerializeField] private Vector3 defaultPosition;
        [SerializeField] private Vector3 selectAdjustPosition;

        private void Update()
        {
            var select = deckSettingManager.CurrentSelect.Value;
            transform.position = Vector3.Lerp
            (
                transform.position,
                select != null ? select.transform.position + selectAdjustPosition : defaultPosition,
                Time.deltaTime * 10
            );
        }
    }
}