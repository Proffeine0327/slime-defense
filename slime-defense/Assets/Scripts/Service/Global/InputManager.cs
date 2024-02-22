using UnityEngine;

public class InputManager : MonoBehaviour
{
    //services
    private CameraManager cameraManager => ServiceProvider.Get<CameraManager>();

    [SerializeField] private bool isMobile;
    private Vector2 lastScreenTouchPosition;

    public Ray TouchRay { get; private set; }

    public bool IsTouchDown { get; private set; }
    public bool IsTouchUp { get; private set; }
    public bool IsTouch { get; private set; }
    public bool IsDragging { get; private set; }
    public Vector2 TouchBeginPosition { get; private set; }
    public Vector2 TouchPosition { get; private set; }

    private void Update()
    {
        if (isMobile)
        {
            if (IsTouchDown) IsTouchDown = false;
            if (IsTouchUp) IsTouchUp = false;

            if (!IsTouch && Input.touchCount > 0)
            {
                TouchBeginPosition = Input.GetTouch(0).position;
                TouchPosition = Input.GetTouch(0).position;
                IsTouchDown = true;
                IsTouch = true;
            }

            if (IsTouch && Input.touchCount == 0)
            {
                IsTouch = false;
                IsTouchUp = true;
            }

            if (IsTouch)
            {
                TouchPosition = Input.GetTouch(0).position;
            }
        }
        else
        {
            IsTouchDown = Input.GetMouseButtonDown(0);
            IsTouch = Input.GetMouseButton(0);
            IsTouchUp = Input.GetMouseButtonUp(0);
            TouchPosition = Input.mousePosition;
            if(IsTouchDown) TouchBeginPosition = Input.mousePosition;
        }

        if(!IsDragging)
        {
            if(IsTouch && Vector2.Distance(TouchBeginPosition, TouchPosition) > 10f) IsDragging = true;
        }
        else
        {
            if(!IsTouch) IsDragging = false;
        }

        TouchRay = cameraManager.MainCamera.ScreenPointToRay(TouchPosition);
    }
}