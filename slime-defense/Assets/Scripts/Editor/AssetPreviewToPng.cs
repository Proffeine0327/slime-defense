using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AssetPreviewToPng : EditorWindow
{
    [MenuItem("Tools/Prefab View To Png")]
    public static void Convert()
    {
        var directory = EditorUtility.OpenFolderPanel("Select Save Folder", Application.dataPath, "");

        var size = new Vector2Int(512, 512);
        var camera = Camera.main;
        var rt = new RenderTexture(size.x, size.y, 48);
        camera.targetTexture = rt;

        var selects = Selection.gameObjects;
        for (int i = 0; i < selects.Length; i++)
        {
            if(i - 1 >= 0) selects[i - 1].SetActive(false);
            selects[i].SetActive(true);
            var screenShoot = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShoot.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
            screenShoot.Apply();
            SaveTextureToPNGFile(screenShoot, directory, selects[i].name);
        }

        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
    }

    public static void SaveTextureToPNGFile(Texture2D texture, string directory, string file)
    {
        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(directory + '/' + file + ".png", bytes);
        Debug.Log("File was succefully save at: " + directory + '/' + file + ".png");
    }
}