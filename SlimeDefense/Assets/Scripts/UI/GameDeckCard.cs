using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameDeckCard : PopupTrigger, IPointerClickHandler
{
    //inject
    private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();
    private InputManager inputManager => ServiceProvider.Get<InputManager>();
    private DataContext dataContext => ServiceProvider.Get<DataContext>();
    private Grids grids => ServiceProvider.Get<Grids>();

    //member
    [SerializeField] private Image profile;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI moneyText;

    private string targetKey;
    private Slime preview;
    
    //property

    public GameDeckCard Init(string key, Explain popup)
    {
        targetKey = key;

        // profile.sprite = targetData.profileSprite;
        // nameText.text = targetData.unitname;
        // moneyText.text = targetData.stats[0].cost.ToString("#,##0");

        // SetPopup(popup);
        popup.Hide();
        return this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.dragging) return;

        var emptyGrids = grids.GetGrids();

        if(emptyGrids.Count == 0)
        {
            //cannot spawn
        }
        else
        {
            var targetGrid = emptyGrids[Random.Range(0, emptyGrids.Count)];
            // if(sliemManager.SpawnUnit(targetKey, targetGrid.XY))
            //     sliemManager.Select(targetGrid.XY);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // preview = Instantiate(targetData.prefab);
        // preview.Init(targetData.name, isPreview: true);
        // grids.DisplayGrids(targetData.placeableState);
        // sliemManager.Select(null);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var plane = new Plane(Vector3.down, Vector3.zero);
        if(plane.Raycast(inputManager.TouchRay, out var dist))
        {
            var hitPoint = inputManager.TouchRay.GetPoint(dist);
            if(grids.PosInGrid(hitPoint)) preview.transform.position = grids.Snap(hitPoint);
            else preview.transform.position = hitPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var plane = new Plane(Vector3.down, Vector3.zero);
        if(plane.Raycast(inputManager.TouchRay, out var dist))
        {
            var hitPoint = inputManager.TouchRay.GetPoint(dist);
            // if(grids.PosInGrid(hitPoint) && sliemManager.SpawnUnit(targetKey, grids.ToIndex(hitPoint)))
            //     sliemManager.Select(grids.ToIndex(hitPoint));
        }

        Destroy(preview.gameObject);
        grids.HideAllGrids();
    }
}
