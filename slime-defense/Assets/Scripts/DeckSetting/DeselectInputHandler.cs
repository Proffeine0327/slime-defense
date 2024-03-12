using Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.DeckSettingScene
{
    public class DeselectInputHandler : MonoBehaviour, IPointerClickHandler
    {
        //services
        private DeckSettingManager deckSettingManager => ServiceProvider.Get<DeckSettingManager>();

        public void OnPointerClick(PointerEventData eventData)
        {
            deckSettingManager.Select(null);
        }
    }
}