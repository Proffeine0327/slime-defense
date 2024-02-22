using System.Collections.Generic;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

public enum ToolType { Brush, Erase, Replace }

[System.Serializable]
public struct PrefabInfo { [ShowAssetPreview] public GameObject prefab; }

public class CubeMap : MonoBehaviour
{
#if UNITY_EDITOR
    public bool editing;
    public ToolType toolType;
    public PrefabInfo[] prefabs;
    public int prefabIndex;
    public int depth;
    public Vector3 scale = Vector3.one;
    public Vector3 rotate = Vector3.zero;
    [HideInInspector] public GameObject preview;
    [HideInInspector] public SerializableDictionary<Vector3Int, GameObject> blocks = new();
#endif
}