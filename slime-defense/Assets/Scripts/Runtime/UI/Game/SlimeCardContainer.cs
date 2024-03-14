using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UnityEngine;

namespace Game.UI.GameScene
{
    public class SlimeCardContainer : MonoBehaviour
    {
        //service
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [Header("Card")]
        [SerializeField] private SlimeCard[] cards;
        [SerializeField] private RectTransform cardGroup;
        [SerializeField] private Explain explain;

        private void Start()
        {
            // foreach(var key in dataContext.userData.saveData.deck)
            for(int i = 0; i < cards.Length; i++)
                cards[i].Init(dataContext.userData.saveData.deck[i]);
        }
    }
}