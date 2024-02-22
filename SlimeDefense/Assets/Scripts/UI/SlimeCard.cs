using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class SlimeCard : PopupTrigger, IPointerClickHandler
    {
        //service
        private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();
        private InputManager inputManager => ServiceProvider.Get<InputManager>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();
        private Grids grids => ServiceProvider.Get<Grids>();

        //member
        [SerializeField] private Image background;
        [SerializeField] private Image border;
        [SerializeField] private Image profile;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI moneyText;

        private string slimeKey;
        private Slime preview;

        //property
        private SlimeData data => dataContext.slimeDatas[slimeKey];

        //method
        public void Init(string key)
        {
            slimeKey = key;

            Debug.Log(data.tier);
            background.color = dataContext.tierDatas[data.tier].color;
            border.color = dataContext.tierDatas[data.tier].color;
            // profile.sprite = resourceLoader.slimeProfiles[slimeKey];
            nameText.text = data.name;
            moneyText.text = data.cost.ToString("#,##0");

            popup.Hide();
            Debug.Log("Initialize");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;

            var emptyGrids = grids.GetGrids();

            if (emptyGrids.Count == 0)
            {
                //cannot spawn
            }
            else
            {
                var targetGrid = emptyGrids[Random.Range(0, emptyGrids.Count)];
                if(slimeManager.SpawnUnit(slimeKey, targetGrid.XY))
                    slimeManager.Select(targetGrid.XY);
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
            if (plane.Raycast(inputManager.TouchRay, out var dist))
            {
                var hitPoint = inputManager.TouchRay.GetPoint(dist);
                if (grids.PosInGrid(hitPoint)) preview.transform.position = grids.Snap(hitPoint);
                else preview.transform.position = hitPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var plane = new Plane(Vector3.down, Vector3.zero);
            if (plane.Raycast(inputManager.TouchRay, out var dist))
            {
                var hitPoint = inputManager.TouchRay.GetPoint(dist);
                // if(grids.PosInGrid(hitPoint) && sliemManager.SpawnUnit(targetKey, grids.ToIndex(hitPoint)))
                //     sliemManager.Select(grids.ToIndex(hitPoint));
            }

            Destroy(preview.gameObject);
            grids.HideAllGrids();
        }
    }
}