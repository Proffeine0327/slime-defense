using UnityEngine;

namespace Game.Services
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera { get; private set; }

        private void Awake()
        {
            ServiceProvider.Register(this);

            mainCamera = GetComponent<Camera>();
        }
    }
}