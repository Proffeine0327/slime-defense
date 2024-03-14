using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.Services;

namespace Game.UI
{
    public class PopupTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] protected Image image;
        [SerializeField] protected Popup popup;
        [SerializeField] protected bool changeLayer;
        
        protected Transform popupParent;

        private Image Image => image ??= GetComponent<Image>();

        public Sprite Sprite
        {
            get => Image.sprite;
            set
            {
                if(value == null) return;
                Image.sprite = value;
            }
        }
        public Popup PopupWindow => popup;
        public event System.Action<bool> OnChangeState;

        private void Start()
        {
            popupParent = popup?.transform.parent;
            popup?.Hide();
        }

        public virtual void SetPopup(Popup popup)
        {
            if (!popup) return;
            popupParent = popup.transform.parent;
            this.popup = popup;
        }

        private void OnDestroy()
        {
            if (popup) Destroy(popup);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnChangeState?.Invoke(true);
            if (popup)
            {
                // if(changeLayer)
                //     popup.transform.SetParent(canvasManager.GetLayer(1).transform);
                popup.Display();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnChangeState?.Invoke(false);
            if (popup)
            {
                // if(changeLayer)
                //     popup.transform.SetParent(popupParent);
                popup.Hide();
            }
        }
    }
}