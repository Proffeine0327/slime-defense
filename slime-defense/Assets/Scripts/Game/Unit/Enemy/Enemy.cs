using System.Collections;
using UnityEngine;

public partial class Enemy : UnitBase
{
    //services
    private Paths paths => ServiceProvider.Get<Paths>();

    private Animator animator;

    public bool IsDeath { get; private set; }
    public float Distance { get; private set; }

    public void Damage(float amount)
    {
        
    }

    private IEnumerator MovePath(int pathIndex)
    {
        var path = paths.GetPath(pathIndex);
        var targetIndex = 1;

        transform.position = path.GetPathPoint(0);

        while (path.MaxPointCount > targetIndex)
        {
            if (IsDeath == true) yield break;

            var diff = transform.position - path.GetPathPoint(targetIndex);
            var rotate = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotate - 180, 0);

            // Distance += Time.deltaTime * Data.speed;
            if (Vector3.Distance(transform.position, path.GetPathPoint(targetIndex)) < 0.01f)
                transform.position = path.GetPathPoint(targetIndex++);
            yield return null;
        }

        IsDeath = true;
        // dataManager.GameData.life -= 1;
        Destroy(gameObject);
    }
}