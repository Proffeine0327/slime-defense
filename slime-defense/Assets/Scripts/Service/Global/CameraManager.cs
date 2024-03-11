using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Services
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera { get; private set; }

        private void Awake()
        {
            ServiceProvider.Register(this, true);

            SceneManager.activeSceneChanged += (_, _) => mainCamera = Camera.main;
        }
    }
}