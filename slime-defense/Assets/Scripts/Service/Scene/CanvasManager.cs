using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private void Awake()
    {
        ServiceProvider.Register(this);
    }
}