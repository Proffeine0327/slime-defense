using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private void OnSceneGUI()
    {
        var displayPositionHandle = serializedObject.FindProperty("displayPositionHandle").boolValue;
        var points = serializedObject.FindProperty("points");
        var position = (target as Path).transform.position;

        if (displayPositionHandle)
        {
            for (int i = 0; i < points.arraySize; i++)
            {
                var point = points.GetArrayElementAtIndex(i);
                point.vector3Value = Handles.PositionHandle(position + point.vector3Value, Quaternion.identity) - position;

                var labelstyle = new GUIStyle(GUI.skin.label);
                labelstyle.fontSize = 15;
                labelstyle.normal.textColor = Color.black;
                Handles.Label(position + point.vector3Value, $"Point {i}", labelstyle);
            }
        }

        for (int i = 0; i < points.arraySize - 1; i++)
        {
            var curPoint = points.GetArrayElementAtIndex(i);
            var nextPoint = points.GetArrayElementAtIndex(i + 1);

            Handles.color = Color.magenta;
            Handles.DrawLine(position + curPoint.vector3Value, position + nextPoint.vector3Value, 3);
            Handles.color = Color.white;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class Path : MonoBehaviour
{
    [SerializeField] private bool displayPositionHandle;
    [SerializeField] private List<Vector3> points = new();

    public Vector3 GetPathPoint(int index) => points[index] + transform.position;
    public int MaxPointCount => points.Count;
}
