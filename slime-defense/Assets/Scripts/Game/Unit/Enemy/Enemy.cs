using System;
using System.Collections;
using UnityEngine;

public partial class Enemy : UnitBase
{
    //services
    private Paths paths => ServiceProvider.Get<Paths>();
    private DataContext dataContext => ServiceProvider.Get<DataContext>();

    private string key;
    private Animator animator;

    protected override Stats BaseStats => dataContext.enemyDatas[key].baseStat;
    public bool IsDisabled { get; private set; }
    public float Distance { get; private set; }

    public event Action OnDeath;
    public event Action OnArrive;

    protected override void Initialize()
    {
        base.Initialize();

        animator = new(this);

        curStats.OnStatChanged += (key, pre, cur) =>
        {
            if (key != Stats.Key.Hp) return;
            Debug.Log($"Damaged {pre - cur}");
        };
        IsDisabled = true;
    }

    public void Appeare(int lv)
    {
        Debug.Log($"lv: {lv}");
        maxStats.ChangeFrom(BaseStats);
        effects.AddOrChange("EnemyUpgrade", new EnemyUpgradeEffect() { key = key, lv = lv });
        curStats.ChangeFrom(maxStats);
        StartCoroutine(MovePath(0));
    }

    public void Damage(float damage)
    {
        skill.OnDamage(damage);
    }

    private void Update()
    {
        if (!IsDisabled && curStats.GetStat(Stats.Key.Hp) <= 0)
        {
            IsDisabled = true;
            animator.PlayDeath();
            OnDeath?.Invoke();
            this.Invoke(() =>
            {
                if(IsDisabled)
                    gameObject.SetActive(false);
            }, 3f);
        }
    }

    private IEnumerator MovePath(int pathIndex)
    {
        animator.PlayMove();

        var path = paths.GetPath(pathIndex);
        transform.position = path.GetPathPoint(pathIndex);

        yield return new WaitForSeconds(0.5f);

        IsDisabled = false;
        var targetIndex = 1;
        while (path.MaxPointCount > targetIndex)
        {
            if (IsDisabled) yield break;

            var diff = transform.position - path.GetPathPoint(targetIndex);
            var rotate = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotate - 180, 0);

            transform.position = Vector3.MoveTowards
                (
                    transform.position,
                    path.GetPathPoint(targetIndex),
                    curStats.GetStat(Stats.Key.Speed) * Time.deltaTime
                );

            Distance += Time.deltaTime * curStats.GetStat(Stats.Key.Speed);
            if (Vector3.Distance(transform.position, path.GetPathPoint(targetIndex)) < 0.01f)
                transform.position = path.GetPathPoint(targetIndex++);
            yield return null;
        }

        IsDisabled = true;
        gameObject.SetActive(false);
        OnArrive?.Invoke();
    }
}