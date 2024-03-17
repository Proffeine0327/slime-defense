using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.GameScene;

namespace Game.Services
{
    public class SelectManager : MonoBehaviour, IPointerClickHandler
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private Grids grids => ServiceProvider.Get<Grids>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        private ReactiveProperty<ISelectable> select = new();

        public ISelectable CurrentSelect => select.Value;
        public event Action<ISelectable> OnSelect;

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

        public void Remove()
        {
            gameManager.SaveData.money += select.Value.RemoveCost;
            select.Value.OnRemove();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select(null);
        }
    }
}