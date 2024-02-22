using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera MainCamera { get; private set; }
    
    private void Awake()
    {
        ServiceProvider.Register(this);

        MainCamera = GetComponent<Camera>();
    }
}