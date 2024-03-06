using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Game.GameScene
{
    public class Grids : MonoBehaviour
    {
        [Serializable]
        public class GridArray
        {
            [SerializeField] private List<Grid> grids = new List<Grid>();
            public List<Grid> Grids => grids;
        }

        [SerializeField] private Vector2Int xySize;
        [SerializeField] private List<GridArray> arrays = new List<GridArray>();

        public Vector2Int MapSize => xySize;
        public List<GridArray> Arrays => arrays;

        public Grid GetGrid(Vector2Int xy)
            => GetGrid(xy.x, xy.y);

        public Grid GetGrid(int x, int y)
            => arrays[y].Grids[x];

        public List<Grid> GetGrids()
        {
            var list = new List<Grid>();
            foreach (var a in arrays) foreach (var grid in a.Grids)
                    list.Add(grid);

            return list;
        }

        public List<Grid> GetGrids(Func<Grid, bool> criteria)
        {
            var list = new List<Grid>();
            foreach (var a in arrays) foreach (var grid in a.Grids)
                    if (criteria(grid)) list.Add(grid);

            return list;
        }

        public Vector2Int ToIndex(Vector3 pos)
        {
            var vec2 = new Vector2(pos.x, pos.z);
            return Vector2Int.RoundToInt(vec2);
        }

        public Vector3 ToPos(Vector2Int index)
        {
            return new Vector3(index.x, transform.position.y, index.y);
        }

        public Vector3 Snap(Vector3 hitPoint)
        {
            Vector3 snapped = Vector3Int.RoundToInt(hitPoint);
            snapped.y = transform.position.y;
            return snapped;
        }

        public bool PosInGrid(Vector3 worldPos)
        {
            var rect = new Rect(Vector2.one * -0.5f, xySize + Vector2.one * 0.5f);
            return rect.Contains(new Vector2(worldPos.x, worldPos.z));
        }

        public bool IndexInGrid(Vector2Int xy)
        {
            return xy.x >= 0 && xy.x < xySize.x && xy.y >= 0 && xy.y < xySize.y;
        }

        public void DisplayGrids(GridType type)
        {
            foreach (var grid in GetGrids())
            {
                grid.Display(type);
            }
        }

        public void HideAllGrids()
        {
            foreach (var grid in GetGrids())
                grid.Hide();
        }

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

        private void Start()
        {
            HideAllGrids();
        }
    }
}