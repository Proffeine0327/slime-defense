using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private CanvasManager canvasManager => ServiceProvider.Get<CanvasManager>();

    [SerializeField] protected Popup popup;
    [SerializeField] protected bool changeLayer;
    protected Image image;
    protected Transform popupParent;

    public Image Image { get { if (!image) image = GetComponent<Image>(); return image; } }
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