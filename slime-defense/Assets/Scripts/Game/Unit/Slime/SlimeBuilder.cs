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

            public Slime BuildOnlyData()
            {
                Debug.Log(dataContext.slimeDatas[slimeKey].skillKey);
                var slime = Instantiate(resourceLoader.slimePrefabs[slimeKey]);
                slime.slimeKey = slimeKey;
                slime.lv.Value = 1;
                slime.Initialize();
                slime.skill = SkillBase.GetSkill(dataContext.slimeDatas[slimeKey].skillKey, slime);
                slime.gameObject.SetActive(false);
                slime.enabled = false;
                slime.gameObject.name = "data";
                return slime;
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
                slime.slimeKey = slimeKey;
                slime.lv.Value = lv;
                slime.isPreview = preview;
                slime.skill = SkillBase.GetSkill(dataContext.slimeDatas[slimeKey].skillKey, slime);
                slime.MoveTo(index);
                slime.Initialize();
                return slime;
            }
        }
    }
}