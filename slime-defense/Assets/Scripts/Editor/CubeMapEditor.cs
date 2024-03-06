using UnityEngine;
using UnityEditor;
using System;
using UniRx;
using Game.GameScene;

[CustomEditor(typeof(CubeMap))]
public class CubeMapEditor : Editor
{
    private ReactiveProperty<bool> editing = new();
    private ReactiveProperty<Vector3Int> mouseDragWorldPos = new();
    private ReactiveProperty<int> prefabIndex = new();
    private ReactiveProperty<ToolType> toolType = new();
    private Material previewMat;
    private Material eraseMat;
    private bool isInitialize = true;

    private CubeMap Target => target as CubeMap;

    private void OnEnable()
    {
        previewMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CubeMap/Preview.mat");
        eraseMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CubeMap/Erase.mat");

        editing.Subscribe(value =>
        {
            if (value) SpawnPreview();
            else
            {
                if (Target.preview)
                    DestroyImmediate(Target.preview);
            }
        });

        toolType.Subscribe(_ => { if (editing.Value) SpawnPreview(); });

        prefabIndex.Subscribe(_ => { if (editing.Value) SpawnPreview(); });

        mouseDragWorldPos.Subscribe(value =>
        {
            if (isInitialize) return;

            switch (Target.toolType)
            {
                case ToolType.Brush:
                    if (!Target.blocks.ContainsKey(value))
                        InstantiateBlock(value);
                    break;
                case ToolType.Erase:
                    if (Target.blocks.ContainsKey(value))
                        EraseBlock(value);
                    break;
                case ToolType.Replace:
                    if (!Target.blocks.ContainsKey(value))
                        InstantiateBlock(value);
                    else
                    {
                        EraseBlock(value);
                        InstantiateBlock(value);
                    }
                    break;
            }
        });

        isInitialize = false;
    }

    private void SpawnPreview()
    {
        if (Target.preview) DestroyImmediate(Target.preview);
        Target.preview = Instantiate(Target.prefabs[prefabIndex.Value].prefab, mouseDragWorldPos.Value, Quaternion.identity);

        foreach (var r in GetRenderers(Target.preview))
            switch (toolType.Value)
            {
                case ToolType.Brush:
                    r.material = previewMat;
                    break;
                case ToolType.Erase:
                    r.material = eraseMat;
                    break;
                case ToolType.Replace:
                    r.material = previewMat;
                    break;
            }
    }

    private Renderer[] GetRenderers(GameObject gameObject)
    {
        return gameObject.GetComponentsInChildren<Renderer>();
    }

    private void OnDestroy()
    {
        if (Target.preview) DestroyImmediate(Target.preview);
    }

    private void InstantiateBlock(Vector3Int value)
    {
        var newBlock = PrefabUtility.InstantiatePrefab(Target.prefabs[Target.prefabIndex].prefab) as GameObject;
        newBlock.transform.position = value;
        newBlock.transform.eulerAngles = Target.rotate;
        newBlock.transform.localScale = Target.scale;
        newBlock.transform.SetParent(Target.transform);
        Target.blocks.Add(new(value, newBlock));
        EditorUtility.SetDirty(Target);
    }

    private void EraseBlock(Vector3Int value)
    {
        var eraseBlock = Target.blocks[value];
        Target.blocks.Remove(value);
        DestroyImmediate(eraseBlock);
        EditorUtility.SetDirty(Target);
    }

    private void OnSceneGUI()
    {
        editing.Value = Target.editing;
        if (!Target.editing) return;

        if(Target.preview)
        {
            Target.preview.transform.localScale = Target.scale;
            Target.preview.transform.eulerAngles = Target.rotate;
        }
        prefabIndex.Value = Target.prefabIndex;
        toolType.Value = Target.toolType;

        switch (Event.current.type)
        {
            case EventType.MouseMove:
                {
                    var pos = GetWorldPos(Event.current);
                    var intpos = Vector3Int.RoundToInt(pos);
                    if (Target.preview) Target.preview.transform.position = intpos;
                    break;
                }
            case EventType.MouseDown:
                {
                    if (Event.current.button == 0)
                    {
                        var pos = GetWorldPos(Event.current);
                        var intpos = Vector3Int.RoundToInt(pos);
                        mouseDragWorldPos.SetValueAndForceNotify(intpos);
                        Event.current.Use();
                    }
                    break;
                }
            case EventType.MouseDrag:
                {
                    if (Event.current.button == 0)
                    {
                        var pos = GetWorldPos(Event.current);
                        var intpos = Vector3Int.RoundToInt(pos);
                        if (Target.preview) Target.preview.transform.position = intpos;
                        mouseDragWorldPos.Value = intpos;
                        Event.current.Use();
                    }
                    break;
                }
            case EventType.MouseUp:
                {
                    if (Event.current.button == 0)
                        Event.current.Use();
                    break;
                }
        }
    }

    private Vector3 GetWorldPos(Event e)
    {
        var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        var plane = new Plane(Vector2.down, new Vector3(0, Target.depth, 0));
        plane.Raycast(ray, out float dist);
        var pos = ray.GetPoint(dist);
        return pos;
    }
}