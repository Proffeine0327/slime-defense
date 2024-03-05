using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class SlimeCard : PopupTrigger, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //service
        private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();
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
        private Slime data;
        private Slime preview;

        //property
        private SlimeData slimedata => dataContext.slimeDatas[slimeKey];
        private Explain explain => popup as Explain;

        //method
        public void Init(string key)
        {
            slimeKey = key;

            data = new Slime.Builder(slimeKey).BuildOnlyData();
            data.transform.SetParent(transform);
            Debug.Log(data.transform.parent);

            background.color = dataContext.tierDatas[slimedata.tier].color;
            border.color = dataContext.tierDatas[slimedata.tier].color;
            profile.sprite = resourceLoader.slimeIcons.GetValueOrDefault(slimeKey);
            nameText.text = slimedata.name;
            moneyText.text = slimedata.cost.ToString("#,##0");

            OnChangeState += state =>
            {
                var skilldata = data.skill;
                if (state) explain.Display(skilldata.Icon, skilldata.Name, skilldata.Explain);
                else explain.Hide();
            };
            popup.Hide();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            popup.Hide();
            preview = new Slime.Builder(slimedata.slimeKey)
                .SetPreview()
                .Build();
            grids.DisplayGrids(slimedata.grid);
            selectManager.Select(null);
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
                var posInGrid = grids.PosInGrid(hitPoint);
                var spawnUnit = slimeManager.CreateSlime(slimedata.slimeKey, grids.ToIndex(hitPoint));

                Debug.Log(spawnUnit);

                if (posInGrid && spawnUnit)
                    selectManager.Select(grids.ToIndex(hitPoint));
            }

            Destroy(preview.gameObject);
            grids.HideAllGrids();
        }
    }
}