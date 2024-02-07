using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    public Dictionary<string, GameObject> SlimeModels { get; private set; }

    private void Awake()
    {
        ServiceProvider.Register(this, true);

        

        SlimeModels = new();
        foreach(var model in Resources.LoadAll<GameObject>("Prefab/Slime"))
            SlimeModels.Add(model.name, model);   
    }
}