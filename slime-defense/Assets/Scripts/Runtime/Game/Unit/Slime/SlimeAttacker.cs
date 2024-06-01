using System;
using System.Linq;
using Game.Services;
using UnityEngine;

namespace Game.GameScene
{
    public partial class Slime
    {
        public class Attacker
        {
            //service
            private GameManager gameManager => ServiceProvider.Get<GameManager>();

            private Slime slime;
            private Enemy target;

            public Enemy Target => target;

            public Attacker(Slime slime)
            {
                this.slime = slime;
            }

            private Enemy[] GetEnemiesInRange()
            {
                return gameManager
                    .Enemies
                    .Where(e => Vector3.Distance(slime.transform.position, e.transform.position) <= slime.curStats.GetStat(Stats.Key.AttackRange) + 0.25f)
                    .ToArray();
            }

            public bool HasTarget()
            {
                if (target != null)
                {
                    if (Vector3.Distance(slime.transform.position, target.transform.position) > slime.curStats.GetStat(Stats.Key.AttackRange) + 0.25f)
                        target = null;
                    else if (target.IsDisabled)
                        target = null;
                    else
                        return true;
                }

                return TryFindNewEnemy();
            }

            public bool TryFindNewEnemy()
            {
                var enemies = GetEnemiesInRange();
                if (enemies.Length == 0) return false;

                //get cloest enemy
                var orderedEnemies = enemies
                    .OrderByDescending(e => e.Distance)
                    .ToArray();
                if (orderedEnemies.Length == 0) return false;
                target = orderedEnemies.First();
                return true;
            }
        }
    }
}