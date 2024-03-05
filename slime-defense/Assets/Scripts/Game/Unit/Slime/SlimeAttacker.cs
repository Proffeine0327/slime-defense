using System;
using System.Linq;
using UnityEngine;

public partial class Slime
{
    public class Attacker
    {
        private Slime slime;
        private Enemy target;

        public Enemy Target => target;

        public Attacker(Slime slime)
        {
            this.slime = slime;
        }

        private Collider[] GetEnemiesInRange()
        {
            return Physics
                .OverlapCapsule(
                    slime.transform.position + (Vector3.up * 100),
                    slime.transform.position + (Vector3.down * 100),
                    slime.maxStats.GetStat(Stats.Key.AttackRange),
                    LayerMask.GetMask("Enemy"));
        }

        public bool HasTarget()
        {
            if (target != null)
            {
                if (Vector3.Distance(slime.transform.position, target.transform.position) > slime.curStats.GetStat(Stats.Key.AttackRange) + 0.25f)
                    target = null;
                else if(target.IsDisabled)
                    target = null;
                else
                    return true;
            }
            
            return TryFindNewEnemy();
        }

        public bool TryFindNewEnemy()
        {
            var colliders = GetEnemiesInRange();
            if (colliders.Length == 0) return false;
            var enemies = colliders
                .Select(e => e.GetComponent<Enemy>())
                .Where(e => !e.IsDisabled)
                .OrderByDescending(e => e.Distance)
                .ToArray();
            if(enemies.Length == 0) return false;
            target = enemies.First();
            return true;            
        }
    }
}