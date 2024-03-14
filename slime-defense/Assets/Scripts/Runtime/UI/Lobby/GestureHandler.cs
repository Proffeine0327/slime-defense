using UnityEngine;
using UnityEngine.EventSystems;
using Game.Services;

namespace Game.UI.LobbyScene
{
    public class GestureHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        //services
        private LobbyManager lobbyManager => ServiceProvider.Get<LobbyManager>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        public void OnDrag(PointerEventData eventData)
        {
            if(lobbyManager.IsSelectedStage.Value) return;
            
            if (eventData.delta.x > 20)
                lobbyManager.Stage.Value =
                    Mathf.Clamp(lobbyManager.Stage.Value - 1, 1, dataContext.stageDatas.Count);
            if (eventData.delta.x < -20)
                lobbyManager.Stage.Value =
                    Mathf.Clamp(lobbyManager.Stage.Value + 1, 1, dataContext.stageDatas.Count);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.dragging) return;

            lobbyManager.IsSelectedStage.Value = !lobbyManager.IsSelectedStage.Value;
        }
    }
}