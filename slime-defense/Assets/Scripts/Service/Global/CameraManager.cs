using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Services
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera { get; private set; }

        public Vector2 GetScreenPosition(Transform transform)
        {
            var pos = transform.position;
            return mainCamera.WorldToScreenPoint(pos);
        }

        private void Awake()
        {
            ServiceProvider.Register(this, true);

            SceneManager.sceneUnloaded += _ => mainCamera = null;
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if(!mainCamera || !mainCamera.gameObject.activeSelf)
                mainCamera = Camera.main;
        }
    }
}