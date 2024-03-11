using Game.Services;
using UnityEngine;

namespace Game.DeckSettingScene
{
    public partial class Slime
    {
        public class Builder
        {
            //services
            private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

            private string slimeKey;

            public Builder(string slimeKey)
            {
                this.slimeKey = slimeKey;
            }

            public Slime Build()
            {
                var slime = Instantiate(resourceLoader.deckSlimePrefabs[slimeKey]);
                slime.key = slimeKey;
                slime.enabled = true;
                return slime;
            }
        }
    }
}