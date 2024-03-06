using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Game.GameScene;

namespace Game.Services
{
    public class ResourceLoader : MonoBehaviour
    {
        public Material gridDefaultMaterial;
        public Material gridPlaceableMaterial;
        public Material gridUnplaceableMaterial;
        public Dictionary<string, Slime> slimePrefabs = new();
        public Dictionary<string, Enemy> enemyPrefabs = new();
        public Dictionary<string, Sprite> slimeIcons = new();
        public Dictionary<string, Sprite> skillIcons = new();

        private void Awake()
        {
            ServiceProvider.Register(this, true);

            gridDefaultMaterial = Resources.Load<Material>("Materials/Grid/Default");
            gridPlaceableMaterial = Resources.Load<Material>("Materials/Grid/Placeable");
            gridUnplaceableMaterial = Resources.Load<Material>("Materials/Grid/Unplaceable");

            foreach (var prefab in Resources.LoadAll<Slime>("Prefabs/Slime"))
                slimePrefabs.Add(prefab.name, prefab);

            foreach (var prefab in Resources.LoadAll<Enemy>("Prefabs/Enemy"))
                enemyPrefabs.Add(prefab.name, prefab);

            foreach (var sprite in Resources.LoadAll<Sprite>("Sprites/Slime/Icon"))
                slimeIcons.Add(sprite.name, sprite);
        }
    }
}