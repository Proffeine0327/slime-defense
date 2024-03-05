using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public class GridSettingEditorWindow : EditorWindow
{
    public class TwoDimensionalArrayWrapper<T>
    {
        private List<List<T>> array;

        public readonly int xSize;
        public readonly int ySize;

        public T this[int y, int x]
        {
            get => array[y][x];
            set => array[y][x] = value;
        }

        public TwoDimensionalArrayWrapper(int y, int x)
        {
            this.xSize = x;
            this.ySize = y;
            array = new(y);
            for(int i = 0; i < y; i++)
                array.Add(new List<T>(Enumerable.Repeat<T>(default, x)));
        }

        public T[,] ToArray()
        {
            var newArray = new T[ySize, xSize];
            for(int y = 0; y < ySize; y++) for(int x = 0; x < xSize; x++)
                newArray[y, x] = array[y][x];
            return newArray;
        }

        public void ReverseY()
        {
            array.Reverse();
        }
    }

    public static void ShowWindow(Vector2Int size, Action<Material, int[,]> onSubmit)
    {
        var window = GetWindow<GridSettingEditorWindow>();
        window.titleContent = new GUIContent("GridSettingWindow");
        window.size = size;
        window.table = new TwoDimensionalArrayWrapper<int>(size.y, size.x);
        window.isAlreadyAdded = new bool[size.y, size.x];
        window.onSubmit = onSubmit;
        window.screenshot = TakeScreenShot(size);
        window.Show();
    }

    private static Texture2D TakeScreenShot(Vector2Int size)
    {
        var bigSize = new Vector2Int(size.x * (1500 / size.y), 1500);
        var camera = new GameObject("Camera").AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = size.y / 2f;
        camera.transform.position = new Vector3(size.x / 2f - 0.5f, 30, size.y / 2f - 0.5f);
        camera.transform.rotation = Quaternion.Euler(90, 0, 0);

        var rt = new RenderTexture(bigSize.x, bigSize.y, 48);
        camera.targetTexture = rt;
        var screenShoot = new Texture2D(bigSize.x, bigSize.y, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShoot.ReadPixels(new Rect(0, 0, bigSize.x, bigSize.y), 0, 0);
        screenShoot.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        DestroyImmediate(camera.gameObject);

        return screenShoot;
    }

    private Material material;

    private Vector2Int size;
    private float boxSize = 25f;
    private Vector2 boxOffset;
    private Action<Material, int[,]> onSubmit;
    private TwoDimensionalArrayWrapper<int> table;
    private bool[,] isAlreadyAdded;
    private Texture2D screenshot;

    private void OnGUI()
    {
        var tagbtnstyle = new GUIStyle(EditorStyles.popup);
        tagbtnstyle.alignment = TextAnchor.MiddleLeft;
        tagbtnstyle.fixedWidth = 200;

        material = EditorGUILayout.ObjectField("Material", material, typeof(Material), true, GUILayout.Width(400)) as Material;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Field", new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 });

        // <----------------------- box control ----------------------->
        GUI.color = new Color(1, 1, 1, 0.3f);
        GUI.DrawTexture(new Rect(boxOffset.x, boxOffset.y + 150, size.x * boxSize, size.y * boxSize), screenshot, ScaleMode.StretchToFill);
        GUI.color = Color.white;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (table[y, x] == 0) GUI.backgroundColor = Color.green;
                if (table[y, x] == 1) GUI.backgroundColor = Color.yellow;
                if (table[y, x] == 2) GUI.backgroundColor = Color.red;

                var curRect = new Rect(boxOffset.x + boxSize * x, boxOffset.y + 150 + boxSize * y, boxSize - 1, boxSize - 1);
                if (curRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 0)
                    {
                        if (!isAlreadyAdded[y, x] && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
                        {
                            table[y, x]++;
                            isAlreadyAdded[y, x] = true;
                        }
                        if (table[y, x] > 2) table[y, x] = 0;
                    }
                }

                GUI.Box(curRect, ((GridType)table[y, x]).ToString());
            }
        }

        GUI.backgroundColor = Color.white;
        if (GUI.Button(new Rect(boxOffset.x,boxOffset.y + 150 + boxSize * size.y, 100, 20), "Reset")) for (int i = 0; i < size.y; i++) for (int j = 0; j < size.x; j++) table[i, j] = 0;
        if (GUI.Button(new Rect(boxOffset.x + 120, boxOffset.y + 150 + boxSize * size.y, 100, 20), "Save"))
        {
            var saveobj = new GridSettingDatas();
            saveobj.material = material;

            for (int i = 0; i < table.ySize; i++)
            {
                saveobj.table.Add(new GridSettingDataTableRow());
                for (int j = 0; j < table.xSize; j++) saveobj.table[i].row.Add(table[i, j]);
            }
            var content = JsonUtility.ToJson(saveobj);
            string directory = EditorUtility.OpenFolderPanel("Select Directory", Application.dataPath, "");

            var time = DateTime.Now.ToString("[yyyy-MM-dd HH-mm-ss]");
            File.WriteAllText($"{directory}/GridSetting{time}.json", content);
            AssetDatabase.Refresh();
        }
        if (GUI.Button(new Rect(boxOffset.x + 220, boxOffset.y + 150 + boxSize * size.y, 100, 20), "Load"))
        {
            string directory = EditorUtility.OpenFilePanel("Select Directory", Application.dataPath, "json");
            string content = File.ReadAllText(directory);
            var loadobj = JsonUtility.FromJson<GridSettingDatas>(content);

            material = loadobj.material;

            for (int i = 0; i < Mathf.Min(size.y, loadobj.table.Count); i++)
            {
                for (int j = 0; j < Mathf.Min(size.x, loadobj.table[0].row.Count); j++)
                    table[i, j] = loadobj.table[i].row[j];
            }
        }
        if (GUI.Button(new Rect(boxOffset.x + boxSize * size.x - 100, boxOffset.y + 150 + boxSize * size.y, 100, 20), "Submit"))
        {
            table.ReverseY();
            onSubmit?.Invoke(material, table.ToArray());
            this.Close();
        }

        if (Event.current.type == EventType.MouseUp)
            for (int i = 0; i < size.y; i++) for (int j = 0; j < size.x; j++) isAlreadyAdded[i, j] = false;

        // <----------------------- window control ----------------------->
        if (Event.current.type == EventType.MouseDrag)
        {
            if (Event.current.button == 2) boxOffset += Event.current.delta;
        }

        if (Event.current.type == EventType.ScrollWheel)
        {
            boxSize -= Event.current.delta.y;
            boxSize = Mathf.Clamp(boxSize, 1, 100);
        }

        Repaint();
    }
}

[Serializable]
public class GridSettingDatas
{
    public Material material;
    public string[] tags;
    public List<GridSettingDataTableRow> table = new List<GridSettingDataTableRow>();
}

[Serializable]
public class GridSettingDataTableRow
{
    public List<int> row = new List<int>();
}
#endif