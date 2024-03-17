using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Services;
using System.Text;

namespace Game.GameScene
{
    public partial class Effects : ISaveLoad
    {
        //serivces
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        public readonly UnitBase owner;

        private Dictionary<string, EffectBase> container = new();

        public void AddOrChange(string effectKey, EffectBase effect)
        {
            if (!container.ContainsKey(effectKey))
            {
                container.Add(effectKey, effect);
            }
            else
            {
                container[effectKey].OnRemove();
                container[effectKey] = effect;
            }
            effect.owner = owner;
            effect.OnAdd();
        }

        public void RemoveEffect(string effectKey)
        {
            if (!container.ContainsKey(effectKey)) return;

            container[effectKey].OnRemove();
            container.Remove(effectKey);
        }

        public IReadOnlyDictionary<string, EffectBase> GetContainer()
        {
            return container;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var ef in container)
                sb.Append($"{ef.Key}").Append('\n');
            return sb.ToString();
        }

        public Effects(UnitBase owner)
        {
            this.owner = owner;
            gameManager.OnWaveEnd += RoundEndEvent;
        }

        ~Effects()
        {
            gameManager.OnWaveEnd -= RoundEndEvent;
        }

        private void RoundEndEvent()
        {
            foreach (var c in container)
                c.Value.OnWaveEnd();
        }

        public string Save()
        {
            var effectDatas = new StringListWrapper();
            foreach(var e in container)
                effectDatas.datas.Add($"{e.Key}\'{e.GetType().FullName},{e.Value.Save()}");
            return JsonUtility.ToJson(effectDatas);
        }

        public void Load(string data)
        {
            var effectDatas = JsonUtility.FromJson<StringListWrapper>(data);
            foreach(var e in effectDatas.datas)
            {
                var key = e.Split('\'')[0];
                var effectInfo = e.Split('\'')[1];
                var effectType = Type.GetType(effectInfo.Split(',')[0]);
                var effectJson = effectInfo.Split(',')[1];
                var effect = (EffectBase)Activator.CreateInstance(effectType);
                effect.Load(effectJson);
                container.Add(key, effect);
            }
        }
    }
}