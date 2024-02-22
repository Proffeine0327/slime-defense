using System.Linq;
using UnityEngine;

public partial class Slime
{
    public class SlimeAttacker
    {
        private Slime slime;

        public SlimeAttacker(Slime slime)
        {
            this.slime = slime;
        }

        private Collider[] GetColliderInRange()
        {
            return Physics
                .OverlapCapsule(
                    slime.transform.position + (Vector3.up * 100), 
                    slime.transform.position + (Vector3.down * 100), 
                    slime.maxStats.GetStat(Stats.Key.AttackRange), 
                    LayerMask.GetMask("Enemy"));
        }

        public bool IsEnemyInRange()
        {
            return GetColliderInRange().Any();
        }

        public void AttackEnemy()
        {
            var enemy = GetColliderInRange()
                .Select(c => c.GetComponent<Enemy>())
                .OrderByDescending(e => e.Distance)
                .First();
            enemy.Damage(slime.maxStats.GetStat(Stats.Key.AttackDamage));
        }
    }
}