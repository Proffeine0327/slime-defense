using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeManager : MonoBehaviour
{
    //services
    private Grids grids => ServiceProvider.Get<Grids>();
    
    private void Awake()
    {
        ServiceProvider.Register(this);
    }

    public void Select(Slime slime)
    {
        
    }

    public bool MoveUnit(Vector2Int from, Vector2Int to)
    {
        // if (from == to) return false;

        // var fromUnit = grids.GetGrid(from).Slime;
        // var toUnit = grids.GetGrid(to).Slime;

        // if (grids.GetGrid(to).State != fromUnit.Data.placeableState) return false;
        // if (grids.GetGrid(to).HasObstacle) return false;

        // if (toUnit)
        // {
        //     if (ReferenceEquals(toUnit.Data, fromUnit.Data) && toUnit.Lvl == fromUnit.Lvl && toUnit.Lvl != toUnit.MaxLvl)
        //     {
        //         //Can unit level up
        //         fromUnit.LevelUp();
        //         Destroy(toUnit.gameObject);

        //         grids.GetGrid(to).Slime = grids.GetGrid(from).Slime;
        //         grids.GetGrid(from).Slime = null;

        //         OnUnitUpdate?.Invoke();
        //         return true;
        //     }
        //     else
        //     {
        //         //exchange
        //         toUnit.MoveTo(from);

        //         var temp = grids.GetGrid(from).Slime;
        //         grids.GetGrid(from).Slime = grids.GetGrid(to).Slime;
        //         grids.GetGrid(to).Slime = temp;

        //         OnUnitUpdate?.Invoke();
        //         return true;
        //     }
        // }

        // grids.GetGrid(to).Slime = grids.GetGrid(from).Slime;
        // grids.GetGrid(from).Slime = null;
        // OnUnitUpdate?.Invoke();
        return true;
    }
}