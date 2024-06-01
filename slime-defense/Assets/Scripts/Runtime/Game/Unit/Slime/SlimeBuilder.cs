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

            /// <summary>
            /// build slime basic way
            /// </summary>
            /// <returns>slime instance</returns>
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

            /// <summary>
            /// build disabled slime <br/> 
            /// lv is fixed to 1
            /// </summary>
            /// <returns>slime instance</returns>
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

            /// <summary>
            /// build slime and load from saved string 
            /// </summary>
            /// <param name="json">load string</param>
            /// <returns>slime instance</returns>
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