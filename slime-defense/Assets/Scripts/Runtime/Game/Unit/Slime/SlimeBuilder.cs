using System;
using UnityEngine;
using Game.Services;

namespace Game.GameScene
{
    public partial class Slime
    {
        public class Builder
        {
            //services
            private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();
            private DataContext dataContext => ServiceProvider.Get<DataContext>();

            private bool preview;
            private int lv = 1;
            private string slimeKey;
            private Vector2Int index;

            public Builder(string slimeKey)
            {
                this.slimeKey = slimeKey;
            }

            public Builder SetPreview()
            {
                preview = true;
                return this;
            }

            public Builder SetLv(int lv)
            {
                this.lv = lv;
                return this;
            }

            public Builder SetIndex(Vector2Int index)
            {
                this.index = index;
                return this;
            }

            public Slime Build()
            {
                var slime = Instantiate(resourceLoader.slimePrefabs[slimeKey]);
                slime.enabled = true;
                slime.slimeKey = slimeKey;
                slime.Initialize();
                slime.lv.Value = lv;
                slime.isPreview = preview;
                slime.skill = SkillBase.GetSkill(dataContext.slimeDatas[slimeKey].skillKey, slime);
                slime.MoveTo(index);

                return slime;
            }

            public Slime BuildOnlyData()
            {
                var slime = Instantiate(resourceLoader.slimePrefabs[slimeKey]);
                slime.slimeKey = slimeKey;
                slime.Initialize();
                slime.lv.Value = 1;
                slime.skill = SkillBase.GetSkill(dataContext.slimeDatas[slimeKey].skillKey, slime);
                slime.gameObject.SetActive(false);
                slime.enabled = false;
                slime.gameObject.name = "data";
                return slime;
            }

            public Slime BuildFromData(string json)
            {
                var slime = Instantiate(resourceLoader.slimePrefabs[slimeKey]);
                slime.slimeKey = slimeKey;
                slime.Initialize();
                slime.skill = SkillBase.GetSkill(dataContext.slimeDatas[slimeKey].skillKey, slime);
                slime.Load(json);
                slime.enabled = true;
                return slime;
            }
        }
    }
}