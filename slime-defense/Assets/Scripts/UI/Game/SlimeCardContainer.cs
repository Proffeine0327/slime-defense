using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI.GameScene
{
    public class SlimeCardContainer : MonoBehaviour
    {
        [Header("Card")]
        [SerializeField] private SlimeCard[] cards;
        [SerializeField] private RectTransform cardGroup;
        [SerializeField] private Explain explain;

        private void Start()
        {
            var testKey = new string[]
            {
                "GrassSlime",
                "GrassSlime",
                "GrassSlime",
                "GrassSlime",
                "GrassSlime",
                "GrassSlime",
            };

            // foreach(var key in dataContext.userData.saveData.deck)
            for(int i = 0; i < cards.Length; i++)
                cards[i].Init(testKey[i]);
        }
    }
}