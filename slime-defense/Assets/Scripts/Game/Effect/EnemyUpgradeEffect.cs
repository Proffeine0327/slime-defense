using UnityEngine;
using Game.Services;

namespace Game.GameScene
{
    public class EnemyUpgradeEffect : EffectBase
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        public string key;
        public int lv;

        public override void OnAdd()
        {
            var enemy = owner as Enemy;

            Debug.Log("Enemy Upgrade");
            enemy.modifier.Set
            (
                caster: "EnemyUpgradeHp",
                key: Stats.Key.Hp,
                percent: _ => dataContext.enemyDatas[key].upgradeHpPercentage * (lv - 1),
                add: _ => dataContext.enemyDatas[key].upgradeHpAdd * (lv - 1)
            );
            enemy.modifier.Set
            (
                caster: "EnemyUpgradeSpeed",
                key: Stats.Key.Speed,
                percent: _ => dataContext.enemyDatas[key].upgradeSpeedPercentage * (lv - 1),
                add: _ => dataContext.enemyDatas[key].upgradeSpeedAdd * (lv - 1)
            );
            enemy.modifier.Set
            (
                caster: "EnemyUpgradeAp",
                key: Stats.Key.AbilityPower,
                percent: _ => dataContext.enemyDatas[key].upgradeApPercentage * (lv - 1),
                add: _ => dataContext.enemyDatas[key].upgradeApAdd * (lv - 1)
            );
        }
    }
}