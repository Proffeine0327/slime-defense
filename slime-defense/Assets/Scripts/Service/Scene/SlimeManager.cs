using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.GameScene;

namespace Game.Services
{
    public class SlimeManager : MonoBehaviour
    {
        //services
        private Grids grids => ServiceProvider.Get<Grids>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        public event Action OnUnitUpdate;

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
            // if (grids.GetGrid(to).HasObstacle) return false;

            if (toUnit)
            {
                if (toUnit.SlimeKey == fromUnit.SlimeKey && toUnit.Lv == fromUnit.Lv && toUnit.Lv != dataContext.gameData.maxLv)
                {
                    Debug.Log("upgrade");
                    //Can unit level up
                    fromUnit.LevelUp();
                    Destroy(toUnit.gameObject);

                    grids.GetGrid(to).Slime = grids.GetGrid(from).Slime;
                    grids.GetGrid(from).Slime = null;

                    OnUnitUpdate?.Invoke();
                    return true;
                }
                else
                {
                    Debug.Log("change");
                    //exchange
                    toUnit.MoveTo(from);

                    var temp = grids.GetGrid(from).Slime;
                    grids.GetGrid(from).Slime = grids.GetGrid(to).Slime;
                    grids.GetGrid(to).Slime = temp;

                    OnUnitUpdate?.Invoke();
                    return true;
                }
            }

            Debug.Log("move");
            grids.GetGrid(to).Slime = grids.GetGrid(from).Slime;
            grids.GetGrid(from).Slime = null;
            OnUnitUpdate?.Invoke();
            return true;
        }

        public bool CreateSlime(string slimeKey, Vector2Int xy)
        {
            if(!grids.IndexInGrid(xy)) return false;

            var data = dataContext.slimeDatas[slimeKey];            
            var grid = grids.GetGrid(xy);
            var saveData = dataContext.userData.saveData;

            Debug.Log(saveData.money);
            if (saveData.money < data.cost) return false;
            if (data.grid != grid.Type) return false;

            if (grid.Slime)
            {
                var unit = grid.Slime;
                if (unit.SlimeKey == slimeKey) return false;
                if (unit.Lv != 0) return false;

                saveData.money -= data.cost;
                unit.LevelUp();
                OnUnitUpdate?.Invoke();
                return true;
            }

            var playerUnit = new Slime.Builder(slimeKey)
                .SetIndex(xy)
                .Build();
            grid.Slime = playerUnit;
            saveData.money -= data.cost;
            // SoundManager.PlaySound("Tower_Apperance", 50);
            OnUnitUpdate?.Invoke();
            return true;
        }

        public void SellSlime(Vector2Int xy)
        {
            var grid = grids.GetGrid(xy);
            grid.Slime = null;
        }
    }
}