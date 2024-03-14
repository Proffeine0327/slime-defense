using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UnityEngine;

namespace Game.LobbyScene
{
    public class StageModelFader : MonoBehaviour
    {
        //services
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();

        private int index;
        private float dissolveValue;
        private Material dissolveMat;

        private bool IsSelected => lobbyManager.Stage.Value - 1 == index;

        public void SetIndex(int index)
        {
            this.index = index;
        }

        private void Start()
        {
            dissolveMat = GetComponent<MeshRenderer>().material;
        }
        
        private void Update()
        {
            dissolveValue = Mathf.Lerp(dissolveValue, IsSelected ? 0 : 1, 3 * Time.deltaTime);
            dissolveMat.SetFloat("_Dissolve", dissolveValue);
        }
    }
}