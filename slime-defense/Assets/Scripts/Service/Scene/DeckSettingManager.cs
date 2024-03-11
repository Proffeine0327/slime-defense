using System;
using System.Collections.Generic;
using System.Linq;
using Game.DeckSettingScene;
using UnityEngine;

namespace Game.Services
{
    public class DeckSettingManager : MonoBehaviour
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private Slot[] slots;
        
        private Dictionary<string, Slime> slimes = new();

        private void Awake()
        {
            ServiceProvider.Register(this);

            var count = 0;
            foreach(var slot in slots)
                slot.SetIndex(count++);

            foreach(var data in dataContext.slimeDatas)
            {
                var slime = new Slime.Builder(data.Key).Build();
                var index = dataContext.userData.deck.IndexOf(data.Key);
                if (index != -1)
                    slime.MoveToDeck(slots[index].transform.position);
                slimes.Add(data.Key, slime);
            }
        }

        public void SetDeck(int index, string key)
        {
            slimes[dataContext.userData.deck[index]].RemoveFromDeck(slots[index].transform.position - Vector3.back);
            slimes[key].MoveToDeck(slots[index].transform.position);
            dataContext.userData.deck[index] = key;
        }
    }
}