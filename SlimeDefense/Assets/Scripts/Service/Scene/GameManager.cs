using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private void Awake()
    {
        ServiceProvider.Register(this);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private IEnumerator GameRoutine()
    {
        while (true)
        {
            yield return null;
        }
    }
}