using UnityEngine;
using UnityEngine.EventSystems;
using Game.Services;

namespace Game.UI.LobbyScene
{
    public class GestureHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        //services
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();

        public void OnDrag(PointerEventData eventData)
        {
            if(lobbyManager.IsSelectedStage) return;
            
            if (eventData.delta.x > 20) lobbyManager.Stage--;
            if (eventData.delta.x < -20) lobbyManager.Stage++;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            lobbyManager.IsSelectedStage = !lobbyManager.IsSelectedStage;
        }
    }
}