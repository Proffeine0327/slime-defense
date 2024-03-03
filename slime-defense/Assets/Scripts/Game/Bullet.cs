using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //service
    private ObjectPool objectPool => ServiceProvider.Get<ObjectPool>();

    private Poolable poolable;

    private void Start()
    {
        poolable = GetComponent<Poolable>();
    }

    public void Fire(Enemy target, Action<Enemy> onHit)
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(target, onHit));
    }

    private IEnumerator MoveRoutine(Enemy target, Action<Enemy> onHit)
    {
        while (Vector3.Distance(target.transform.position, transform.position) > 0.5f)
        {
            if (target.IsDisabled)
            {
                poolable.Pool();
                yield break;
            }

            transform.LookAt(target.transform.position);
            transform.Translate(transform.forward * 15 * Time.deltaTime, Space.World);
            yield return null;
        }
        onHit?.Invoke(target);
        poolable.Pool();
    }
}