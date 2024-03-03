using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log(string.Format("{0}, {1}", 10));
    }
}