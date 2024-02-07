using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SlimeData
{
    
}

public class ExcelDataLoader : MonoBehaviour
{
    public Dictionary<string, SlimeData> SlimeData { get; private set; }

    private void Awake()
    {
        ServiceProvider.Register(this, true);

        
    }
}