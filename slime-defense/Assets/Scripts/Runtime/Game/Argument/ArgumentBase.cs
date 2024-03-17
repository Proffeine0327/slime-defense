using System.Collections.Generic;
using Game.Services;
using UnityEngine;

namespace Game.GameScene
{
    public class ArgumentBase : ISaveLoad
    {
        //service
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

        public virtual Sprite Icon => resourceLoader.argumentIcons.GetValueOrDefault(GetType().Name);
        public virtual string Title => "Argument_Base";
        public virtual string Explain => "Argument_Base_Explain";

        public virtual void OnAdd() { }
        public virtual void OnUpdate() { }
        public virtual void OnSlimeUpdate() { }

        public virtual string Save() { return string.Empty; }
        public virtual void Load(string data) { }
    }
}