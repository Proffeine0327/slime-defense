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

    public void Init(int lv)
    {
        maxStats.ChangeFrom(BaseStats);
        effects.AddOrChange("EnemyUpgrade", new EnemyUpgradeEffect() { key = key, lv = lv });
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
            OnDeath?.Invoke();
        }
    }

    private IEnumerator MovePath(int pathIndex)
    {
        yield return null;
        curStats.ModifyStat(Stats.Key.Hp, x => maxStats.GetStat(Stats.Key.Hp));
        IsDisabled = false;

        var path = paths.GetPath(pathIndex);
        var targetIndex = 1;

        transform.position = path.GetPathPoint(0);

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
        OnArrive?.Invoke();
    }
}