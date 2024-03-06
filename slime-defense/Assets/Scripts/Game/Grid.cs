using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using Game.Services;

namespace Game.GameScene
{
    public enum GridType { None, Ground, Road }

    public class Grid : MonoBehaviour
    {
        //service
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

        [SerializeField] private Vector2Int xy;
        [SerializeField] private GridType gridType;

        private MeshRenderer meshRenderer;

        public Vector2Int XY => xy;
        public GridType Type => gridType;
        public Slime Slime { get; set; }

        public void Init(GridType type, Vector2Int xy)
        {
            this.gridType = type;
            this.xy = xy;
        }

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Display(GridType type)
        {
            meshRenderer.sharedMaterial =
                type == gridType ?
                resourceLoader.gridPlaceableMaterial :
                resourceLoader.gridUnplaceableMaterial;
        }

        public void Hide()
        {
            meshRenderer.sharedMaterial = resourceLoader.gridDefaultMaterial;
        }
    }
}