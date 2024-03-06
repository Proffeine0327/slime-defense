using UnityEngine;

namespace Game.Services
{
    public class CanvasManager : MonoBehaviour
    {
        private void Awake()
        {
            ServiceProvider.Register(this);
        }
    }
}