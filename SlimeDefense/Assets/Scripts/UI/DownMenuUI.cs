using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DownMenuUI : MonoBehaviour
{
    //inject
    private DataContext dataContext => ServiceProvider.Get<DataContext>();

    [Header("Card")]
    [SerializeField] private GameDeckCard cardPrefab;
    [SerializeField] private RectTransform cardGroup;
    [Header("Money")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Awake()
    {
        // foreach(var key in dataContext.UserData.gameData.deck)
        //     Instantiate(cardPrefab, cardGroup).Init(key, explainPopup);
    }

    private void Update()
    {
        moneyText.text = $"{dataContext.userData.saveData.money:#,##0}";
    }
}
