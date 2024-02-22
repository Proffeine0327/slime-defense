using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class Slime : UnitBase, ISelectable, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //services
    private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();
    private GameManager gameManager => ServiceProvider.Get<GameManager>();
    private InputManager inputManager => ServiceProvider.Get<InputManager>();
    private DataContext dataContext => ServiceProvider.Get<DataContext>();
    private Grids grids => ServiceProvider.Get<Grids>();

    //field
    private string key;
    private ReactiveProperty<int> lv = new();
    private Vector2Int index;
    private SlimeAnimator animator;
    private SlimeAttacker attacker;
    private SkillBase skill;

    //property
    private SlimeData slimeData => dataContext.slimeDatas[key];
    public Stats lvStat => slimeData.stats[lv.Value];
    public Stats displayStat => maxStats;

    //method
    protected override void Awake()
    {
        base.Awake();
        animator = new(this);
        attacker = new(this);

        lv.Subscribe(_ => CalculateStat());
    }

    private void Update()
    {
        if (currentStats.GetStat(Stats.Key.AttackDelay) < maxStats.GetStat(Stats.Key.AttackDelay))
        {
            currentStats.ModifyStat(Stats.Key.AttackDelay, x => x + Time.deltaTime);
        }
        else
        {
            if (attacker.IsEnemyInRange())
            {
                animator.PlayAttack(slimeData.atkAnimKey);
                currentStats.ModifyStat(Stats.Key.AttackDelay, x => 0);
            }
        }
    }

    public void Attack()
    {
        attacker.AttackEnemy();
    }

    public void LookEnemy(Enemy enemy)
    {
        var diff = enemy.transform.position - transform.position;
        var rotate = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, rotate, 0);
    }

    public void MoveTo(Vector2Int to)
    {
        index = to;
        transform.position = grids.ToPos(to);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging) return;
        slimeManager.Select(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // if (gameManager.IsProgressWave) return;

        slimeManager.Select(this);
        index = grids.ToIndex(transform.position);
        // grids.DisplayGrids(Data.placeableState);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // if (gameManager.IsProgressWave) return;

        var plane = new Plane(Vector3.down, Vector3.zero);
        if (plane.Raycast(inputManager.TouchRay, out var dist))
        {
            var hitPoint = inputManager.TouchRay.GetPoint(dist);
            if (grids.PosInGrid(hitPoint)) transform.position = grids.Snap(hitPoint);
            else transform.position = hitPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // if (gameManager.IsProgressWave) return;

        if (grids.PosInGrid(transform.position))
        {
            var movedIndex = grids.ToIndex(transform.position);

            if (slimeManager.MoveUnit(index, movedIndex)) MoveTo(movedIndex);
            else MoveTo(index);
        }
        else
        {
            MoveTo(index);
        }

        // grids.HideAllGrids();

        slimeManager.Select(this);
    }
}
