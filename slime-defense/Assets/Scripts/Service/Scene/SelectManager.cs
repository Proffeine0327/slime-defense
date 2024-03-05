using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectManager : MonoBehaviour, IPointerClickHandler
{
    //services
    private Grids grids => ServiceProvider.Get<Grids>();

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

    public void OnPointerClick(PointerEventData eventData)
    {
        Select(null);
    }
}