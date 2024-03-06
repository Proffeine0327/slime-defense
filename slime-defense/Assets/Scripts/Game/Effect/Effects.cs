using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Services;

namespace Game.GameScene
{
    public partial class Effects
    {
        //serivces
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        public readonly UnitBase owner;

        private Dictionary<string, EffectBase> container = new();

        public void AddOrChange(string effectKey, EffectBase effect)
        {
            if (!container.ContainsKey(effectKey))
            {
                Debug.Log("Add");
                container.Add(effectKey, effect);
            }
            else
            {
                Debug.Log("Change");
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
                c.Value.OnRoundEnd();
        }
    }
}