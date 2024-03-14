using System;
using System.Collections.Generic;
using System.Linq;
using Game.DeckSettingScene;
using UniRx;
using UnityEngine;

namespace Game.Services
{
    public class DeckSettingManager : MonoBehaviour
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        //member
        [SerializeField] private Slot[] slots;
        
        private Dictionary<string, Slime> slimes = new();
        
        //property
        public ReactiveProperty<Slime> CurrentSelect = new();

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
                    slime.MoveToDeck(index, slots[index].transform.position);
                slimes.Add(data.Key, slime);
            }
        }

        public void Select(Slime slime)
        {
            CurrentSelect.Value = slime;
        }

        public void SetDeck(int index, string key)
        {
            slimes[dataContext.userData.deck[index]].RemoveFromDeck(slots[index].transform.position - Vector3.back * 1.5f);
            slimes[key].MoveToDeck(index, slots[index].transform.position);
            dataContext.userData.deck[index] = key;
        }

        public void ChangeDeck(int from, int to)
        {
            var f = dataContext.userData.deck[from]; 
            var t = dataContext.userData.deck[to];
            dataContext.userData.deck[from] = t;
            dataContext.userData.deck[to] = f;
            slimes[f].MoveToDeck(to, slots[to].transform.position);
            slimes[t].MoveToDeck(from, slots[from].transform.position);
        }
    }
}