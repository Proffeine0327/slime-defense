using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LobbyScene
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private Vector3 stageSelectedPosition;
        [SerializeField] private Vector3 stageSelectedRotation;
        [SerializeField] private Vector3 stageUnselectedPosition;
        [SerializeField] private Vector3 stageUnselectedRotation;
    }
}