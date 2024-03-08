using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.LoadScene
{
    public class PassLoadingScene : MonoBehaviour
    {
        //service
        private TaskWaiter taskWaiter => ServiceProvider.Get<TaskWaiter>();

        private void Update()
        {
            if (taskWaiter.IsEndLoad)
            {
                SceneManager.LoadScene("Lobby");
            }
        }
    }
}