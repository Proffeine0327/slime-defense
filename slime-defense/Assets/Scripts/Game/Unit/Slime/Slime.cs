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
    private ObjectPool objectPool => ServiceProvider.Get<ObjectPool>();
    private Grids grids => ServiceProvider.Get<Grids>();

    //field
    private bool isPreview;
    private bool look;
    private string slimeKey;
    private ReactiveProperty<int> lv = new();
    private Vector2Int index;
    private Animator animator;
    private Attacker attacker;
    private RangeDisplayer rangeDisplayer;

    //property
    private SlimeData SlimeData => dataContext.slimeDatas[slimeKey];
    private bool IsSelected => ReferenceEquals(this, slimeManager.CurrentSelect);

    protected override Stats BaseStats => SlimeData.stats[lv.Value - 1];

    public Stats DisplayStat => curStats;
    public int Lv => lv.Value;
    public string SlimeKey => slimeKey;
    public bool IsMaxLv => dataContext.gameData.maxLv == lv.Value;

    //method
    protected override void Start()
    {
        base.Start();
        Debug.Log("Slime Start");

        animator = new(this);
        attacker = new(this);

        rangeDisplayer = GetComponentInChildren<RangeDisplayer>();

        curStats.OnStatChanged += (key, pre, cur) =>
        {
            if (key != Stats.Key.AttackRange) return;
            rangeDisplayer.SetRange(curStats.GetStat(key));
        };
        lv.Subscribe(x =>
        {
            if(x <= 0) return;
            modifier.CalculateAll(maxStats, BaseStats);
            Debug.Log(curStats.ToString());
        });
    }

    private void Update()
    {
        if (isPreview)
        {
            rangeDisplayer.Active(true);
            return;
        }

        rangeDisplayer.Active(IsSelected);
        if (!gameManager.IsWaveStart) return;

        if (look) LookEnemy(attacker.Target);

        if (curStats.GetStat(Stats.Key.AttackDelay) < maxStats.GetStat(Stats.Key.AttackDelay))
        {
            curStats.ModifyStat(Stats.Key.AttackDelay, x => x + Time.deltaTime);
        }
        else
        {
            if (attacker.HasTarget())
            {
                look = true;
                animator.PlayAttack(SlimeData.atkAnimKey);
                curStats.ModifyStat(Stats.Key.AttackDelay, x => 0);
            }
        }
    }

    public void LevelUp()
    {
        lv.Value++;
    }

    public void Attack()
    {
        look = false;
        if (!string.IsNullOrEmpty(SlimeData.bulletKey))
        {
            var bullet = objectPool.GetObject(SlimeData.bulletKey);
            bullet.transform.position = transform.position;
            bullet.GetComponent<Bullet>().Fire(attacker.Target, target =>
            {
                skill.OnAttack(target);
                objectPool
                    .GetObject
                    (
                        SlimeData.atkParticleKey,
                        attacker.Target.transform.position + Vector3.up * 0.5f
                    )
                    .GetComponent<Particle>()
                    .Play();
            });
        }
        else
        {
            skill.OnAttack(attacker.Target);
            objectPool
                .GetObject
                (
                    SlimeData.atkParticleKey,
                    attacker.Target.transform.position + Vector3.up * 0.5f
                )
                .GetComponent<Particle>()
                .Play();
        }
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
        if (gameManager.IsWaveStart) return;

        slimeManager.Select(this);
        grids.DisplayGrids(SlimeData.grid);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gameManager.IsWaveStart) return;

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
        if (gameManager.IsWaveStart) return;

        if (grids.PosInGrid(transform.position))
        {
            var movedIndex = grids.ToIndex(transform.position);

            if (slimeManager.MoveSlime(index, movedIndex)) MoveTo(movedIndex);
            else MoveTo(index);
        }
        else
        {
            MoveTo(index);
        }

        grids.HideAllGrids();
        slimeManager.Select(this);
    }
}
