using UnityEngine;

public partial class Enemy
{
    public class Builder
    {
        //services
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

        public string key;
        public string skill;

        public Builder(string key)
        {
            this.key = key;
        }

        public Enemy Build()
        {
            var enemy = Instantiate(resourceLoader.enemyPrefabs[key]);
            enemy.key = key;
            enemy.skill = SkillBase.GetSkill(skill, enemy);
            enemy.Initialize();
            return enemy;
        }
    }
}