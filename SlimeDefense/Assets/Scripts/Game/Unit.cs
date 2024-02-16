using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Unit : MonoBehaviour
{
    protected Stat stat = new();
    protected Effect effect = new();

    protected virtual void Awake()
    {
        
    }
}