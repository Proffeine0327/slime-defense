using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;

namespace Game.UI.GameScene
{
public class RemainMoneyDisplayer : MonoBehaviour
{
    //services
    private DataContext dataContext => ServiceProvider.Get<DataContext>();

    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        dataContext.userData.saveData
            .ObserveEveryValueChanged(s => s.money)
            .Subscribe(x => text.text = x.ToString("#,##0"));
    }
}
}