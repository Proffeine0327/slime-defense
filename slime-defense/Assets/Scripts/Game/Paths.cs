using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour
{
    [SerializeField] private Path[] paths;

    public Path GetPath(int pathIndex) => paths[pathIndex];

    private void Awake()
    {
        ServiceProvider.Register(this);
    }
}