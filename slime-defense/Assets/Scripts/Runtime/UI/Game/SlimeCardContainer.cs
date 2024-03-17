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
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        [Header("Card")]
        [SerializeField] private SlimeCard[] cards;
        [SerializeField] private RectTransform cardGroup;
        [SerializeField] private Explain explain;

        private void Start()
        {
            // foreach(var key in gameManager.SaveData.deck)
            for(int i = 0; i < cards.Length; i++)
                cards[i].Init(gameManager.SaveData.deck[i]);
        }
    }
}