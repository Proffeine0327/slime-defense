using UnityEngine;

public static class Vector2IntExtension
{
    public static string Save(this Vector2Int vec)
        => $"{vec.x}\'{vec.y}";
    
    public static Vector2Int Load(this Vector2Int vec, string data)
    {
        var split = data.Split('\'');
        vec = new(int.Parse(split[0]), int.Parse(split[1]));
        return vec;
    }
}