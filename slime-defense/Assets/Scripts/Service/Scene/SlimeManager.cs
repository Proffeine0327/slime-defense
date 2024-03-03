using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlimeManager : MonoBehaviour, IPointerClickHandler
{
    //services
    private Grids grids => ServiceProvider.Get<Grids>();
    private DataContext dataContext => ServiceProvider.Get<DataContext>();
    private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

    private ReactiveProperty<ISelectable> select = new();

    public event Action OnUnitUpdate;
    public event Action<ISelectable> OnSelect;

    public ISelectable CurrentSelect => select.Value;
    
    private void Awake()
    {
        ServiceProvider.Register(this);
        
        select.Subscribe(select => OnSelect?.Invoke(select));
    }

    public void Select(ISelectable selectable)
    {
        select.Value = selectable;
    }

    public void Select(Vector2Int xy)
    {
        select.Value = grids.GetGrid(xy).Slime;
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
        var data = dataContext.slimeDatas[slimeKey];
        var gameData = dataContext.userData.saveData;

        Debug.Log(gameData.money);
        if (gameData.money < data.cost) return false;

        if (grids.GetGrid(xy).Slime)
        {
            var unit = grids.GetGrid(xy).Slime;
            if (unit.SlimeKey == slimeKey) return false;
            if (unit.Lv != 0) return false;

            gameData.money -= data.cost;
            unit.LevelUp();
            OnUnitUpdate?.Invoke();
            return true;
        }

        var slimeData = dataContext.slimeDatas[slimeKey];
        var playerUnit = new Slime.Builder(slimeKey)
            .SetIndex(xy)
            .Build();
        grids.GetGrid(xy).Slime = playerUnit;
        gameData.money -= data.cost;
        // SoundManager.PlaySound("Tower_Apperance", 50);
        OnUnitUpdate?.Invoke();
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select(null);
    }
}