using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

public enum GridType { None, Ground, Road }

public class Grid : MonoBehaviour
{
    [SerializeField] private Vector2Int xy;
    [SerializeField] private GridType gridType;

    public Vector2Int XY => xy;
    public Slime Slime { get; set; }

    public void Init(GridType type, Vector2Int xy)
    {
        this.gridType = type;
        this.xy = xy;
    }

    public void Display(GridType type)
    {
        
    }

    public void Hide()
    {
        
    }
}
