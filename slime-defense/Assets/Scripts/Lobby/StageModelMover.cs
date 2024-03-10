using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;

namespace Game.LobbyScene
{
    public class StageModelMover : MonoBehaviour
    {
        //services
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();

        [SerializeField] private float dist;
        [SerializeField] private StageModelFader[] models;

        private void Start()
        {
            for(int i = 0; i < models.Length; i++)
                models[i].SetIndex(i);
        }

        private void Update()
        {
            for(int i = 0; i < models.Length; i++)
            {
                models[i].transform.localPosition =
                    Vector3.Lerp
                    (
                        models[i].transform.localPosition, 
                        new Vector3(dist * (lobbyManager.Stage.Value - 1 - i), 0, 0), 
                        5 * Time.deltaTime
                    );
            }
        }
    }
}
