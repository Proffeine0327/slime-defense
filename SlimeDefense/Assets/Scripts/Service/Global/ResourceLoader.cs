using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    public Dictionary<string, Slime> slimeModels = new();

    private void Awake()
    {
        ServiceProvider.Register(this, true);

        foreach(var model in Resources.LoadAll<Slime>("Prefab/Slime"))
            slimeModels.Add(model.name, model);   
    }
}