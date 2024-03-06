using System.Collections;
using System.Collections.Generic;
using Game.GameScene;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grids))]
public class GridsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var xysize = serializedObject.FindProperty("xySize");
        if (GUILayout.Button("Spawn Grid"))
        {
            var xysizevector = xysize.vector2IntValue;
            GridSettingEditorWindow.ShowWindow(xysizevector, (material, states) =>
            {
                var comp = (target as Grids);
                var cols = comp.Arrays;

                for (int i = cols.Count - 1; i >= 0; i--)
                {
                    var rows = cols[i].Grids;
                    for (int j = rows.Count - 1; j >= 0; j--)
                        if (rows[j] != null) DestroyImmediate(rows[j].gameObject);
                }
                cols.Clear();

                for (int y = 0; y < xysizevector.y; y++)
                {
                    cols.Add(new());
                    for (int x = 0; x < xysizevector.x; x++)
                    {
                        var obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        obj.transform.localScale = Vector3.one * 0.095f;
                        obj.transform.SetParent((target as Grids).transform);
                        obj.transform.localPosition = new Vector3(x, 0, y);
                        obj.name = $"Grid ({x},{y})";

                        obj.GetComponent<MeshRenderer>().sharedMaterial = material;

                        DestroyImmediate(obj.GetComponent<MeshCollider>());
                        obj.AddComponent<BoxCollider>();
                        obj.layer = LayerMask.NameToLayer("Grid");

                        var gridplane = obj.AddComponent<Game.GameScene.Grid>();
                        gridplane.Init((GridType)states[y, x], new Vector2Int(x, y));

                        cols[y].Grids.Add(gridplane);
                    }
                }
                EditorUtility.SetDirty(target);
            });
        }
        if (GUILayout.Button("Clear"))
        {
            var comp = target as Grids;
            var cols = comp.Arrays;
            for (int i = cols.Count - 1; i >= 0; i--)
            {
                var rows = cols[i].Grids;
                for (int j = rows.Count - 1; j >= 0; j--)
                    if (rows[j] != null) DestroyImmediate(rows[j].gameObject);
            }
            cols.Clear();
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.Space(10);
        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif