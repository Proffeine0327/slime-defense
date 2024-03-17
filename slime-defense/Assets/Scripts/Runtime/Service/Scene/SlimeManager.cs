using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.GameScene;
using System.IO;

namespace Game.Services
{
    public class SlimeManager : MonoBehaviour, ISaveLoad, IInitialize
    {
        //services
        private Grids grids => ServiceProvider.Get<Grids>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();

        public HashSet<Slime> Slimes = new();

        public event Action OnSlimeUpdate;

        private void Awake()
        {
            ServiceProvider.Register(this);
        }

        public bool MoveSlime(Vector2Int from, Vector2Int to)
        {
            if (from == to) return false;

            var fromUnit = grids.GetGrid(from).Slime;
            var toUnit = grids.GetGrid(to).Slime;

            if (grids.GetGrid(to).Type != dataContext.slimeDatas[fromUnit.SlimeKey].grid) return false;
            if (grids.GetGrid(to).HasObstacle) return false;

            if (toUnit)
            {
                if (toUnit.SlimeKey == fromUnit.SlimeKey && toUnit.Lv == fromUnit.Lv && toUnit.Lv != dataContext.gameData.maxLv)
                {
                    //Can unit level up
                    fromUnit.LevelUp();
                    Destroy(toUnit.gameObject);
                    Slimes.Remove(toUnit);

                    grids.GetGrid(to).Slime = grids.GetGrid(from).Slime;
                    grids.GetGrid(from).Slime = null;

                    OnSlimeUpdate?.Invoke();
                    return true;
                }
                else
                {
                    //exchange
                    toUnit.MoveTo(from);

                    var temp = grids.GetGrid(from).Slime;
                    grids.GetGrid(from).Slime = grids.GetGrid(to).Slime;
                    grids.GetGrid(to).Slime = temp;

                    OnSlimeUpdate?.Invoke();
                    return true;
                }
            }

            grids.GetGrid(to).Slime = grids.GetGrid(from).Slime;
            grids.GetGrid(from).Slime = null;
            OnSlimeUpdate?.Invoke();
            return true;
        }

        public bool CreateSlime(string slimeKey, Vector2Int xy)
        {
            if (!grids.IndexInGrid(xy)) return false;

            var data = dataContext.slimeDatas[slimeKey];
            var grid = grids.GetGrid(xy);
            var saveData = gameManager.SaveData;

            if (saveData.money < data.cost) return false;
            if (data.grid != grid.Type) return false;
            if (grid.HasObstacle) return false;

            if (grid.Slime)
            {
                var unit = grid.Slime;
                if (unit.SlimeKey != slimeKey) return false;
                if (unit.Lv != 1) return false;

                saveData.money -= data.cost;
                unit.LevelUp();
                OnSlimeUpdate?.Invoke();
                return true;
            }

            var slime = new Slime.Builder(slimeKey)
                .SetIndex(xy)
                .Build();
            grid.Slime = slime;
            saveData.money -= data.cost;
            Slimes.Add(slime);
            // SoundManager.PlaySound("Tower_Apperance", 50);
            OnSlimeUpdate?.Invoke();
            return true;
        }

        public void SellSlime(Vector2Int xy)
        {
            var grid = grids.GetGrid(xy);
            Slimes.Remove(grid.Slime);
            Destroy(grid.Slime.gameObject);
            grid.Slime = null;
            selectManager.Select(null);
            OnSlimeUpdate?.Invoke();
        }

        public void Initialize()
        {
            
        }

        public string Save()
        {
            var slimes = new StringListWrapper();
            foreach (var s in Slimes)
                slimes.datas.Add($"{s.SlimeKey}\'{s.Save()}");
            return JsonUtility.ToJson(slimes);
        }

        public void Load(string json)
        {
            var slimes = JsonUtility.FromJson<StringListWrapper>(json);
            foreach (var s in slimes.datas)
            {
                var slimeKey = s[0..s.IndexOf('\'')];
                var slimeJson = s[(s.IndexOf('\'')+1)..];
                var slime = new Slime.Builder(slimeKey)
                    .BuildFromData(slimeJson);
                grids.GetGrid(slime.XY).Slime = slime;
                Slimes.Add(slime);
            }
        }
    }
}