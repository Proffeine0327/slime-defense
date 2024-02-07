using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    private void Awake()
    {
        ServiceProvider.Register(this);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("Development");
    }
}