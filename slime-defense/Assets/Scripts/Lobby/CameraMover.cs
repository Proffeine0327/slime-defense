using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Services;

namespace Game.LobbyScene
{
    public class CameraMover : MonoBehaviour
    {
        //services
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();

        [SerializeField] private float moveSpeed;
        [SerializeField] private Vector3 stageSelectedPosition;
        [SerializeField] private Vector3 stageSelectedRotation;
        [SerializeField] private Vector3 stageUnselectedPosition;
        [SerializeField] private Vector3 stageUnselectedRotation;

        private void Update()
        {
            transform.position = Vector3.Lerp
            (
                transform.position,
                lobbyManager.IsSelectedStage.Value ? stageSelectedPosition : stageUnselectedPosition,
                Time.deltaTime * moveSpeed
            );
            transform.eulerAngles = Vector3.Lerp
            (
                transform.eulerAngles,
                lobbyManager.IsSelectedStage.Value ? stageSelectedRotation : stageUnselectedRotation,
                Time.deltaTime * moveSpeed  
            );
        }
    }
}