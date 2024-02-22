using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassLoadingScene : MonoBehaviour
{
    //service
    private TaskWaiter taskWaiter => ServiceProvider.Get<TaskWaiter>();

    private void Update()
    {
        Debug.Log(taskWaiter.Progress);
        if(taskWaiter.IsEndLoad)
        {
            SceneManager.LoadScene("development");
        }
    }
}
