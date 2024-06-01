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

        /// <summary>
        /// this method called when argument is selected
        /// </summary>
        public virtual void OnAdd() { }

        /// <summary>
        /// this method called when update method called
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// this method called when slime filed changes
        /// </summary>
        public virtual void OnSlimeUpdate() { }

        public virtual string Save() { return string.Empty; }
        public virtual void Load(string data) { }
    }
}