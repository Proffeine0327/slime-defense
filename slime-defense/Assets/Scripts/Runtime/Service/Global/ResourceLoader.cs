using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Game.GameScene;
using System;

namespace Game.Services
{
    public class ResourceLoader : MonoBehaviour
    {
        [NonSerialized] public Material gridDefaultMaterial;
        [NonSerialized] public Material gridPlaceableMaterial;
        [NonSerialized] public Material gridUnplaceableMaterial;
        [NonSerialized] public Dictionary<string, Game.GameScene.Slime> slimePrefabs = new();
        [NonSerialized] public Dictionary<string, Game.DeckSettingScene.Slime> deckSlimePrefabs = new();
        [NonSerialized] public Dictionary<string, Enemy> enemyPrefabs = new();
        [NonSerialized] public Dictionary<string, Sprite> slimeIcons = new();
        [NonSerialized] public Dictionary<string, Sprite> enemyIcons = new();
        [NonSerialized] public Dictionary<string, Sprite> skillIcons = new();

        private void Awake()
        {
            ServiceProvider.Register(this, true);

            gridDefaultMaterial = Resources.Load<Material>("Materials/Grid/Default");
            gridPlaceableMaterial = Resources.Load<Material>("Materials/Grid/Placeable");
            gridUnplaceableMaterial = Resources.Load<Material>("Materials/Grid/Unplaceable");

            foreach (var prefab in Resources.LoadAll<Game.GameScene.Slime>("Prefabs/Slime"))
                slimePrefabs.Add(prefab.name, prefab);

            foreach (var prefab in Resources.LoadAll<Game.DeckSettingScene.Slime>("Prefabs/Slime"))
                deckSlimePrefabs.Add(prefab.name, prefab);

            foreach (var prefab in Resources.LoadAll<Enemy>("Prefabs/Enemy"))
                enemyPrefabs.Add(prefab.name, prefab);

            foreach (var sprite in Resources.LoadAll<Sprite>("Sprites/Slime/Icon"))
                slimeIcons.Add(sprite.name, sprite);

            foreach (var sprite in Resources.LoadAll<Sprite>("Sprites/Enemy/Icon"))
                enemyIcons.Add(sprite.name, sprite);
        }
    }
}