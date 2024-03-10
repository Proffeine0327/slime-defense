using UnityEngine;
using Game.Services;

namespace Game.GameScene
{
    public partial class Enemy
    {
        public class Builder
        {
            //services
            private DataContext dataContext => ServiceProvider.Get<DataContext>();
            private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

            public string key;

            public Builder(string key)
            {
                this.key = key;
            }

            public Enemy Build()
            {
                var enemy = Instantiate(resourceLoader.enemyPrefabs[key]);
                enemy.key = key;
                enemy.skill = SkillBase.GetSkill(dataContext.enemyDatas[key].skillKey, enemy);
                enemy.Initialize();
                return enemy;
            }
        }
    }
}