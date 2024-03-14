using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GameScene
{
    public class TopInfomationController : MonoBehaviour
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Button settingButton;

        private void Start()
        {
            dataContext.userData
                .ObserveEveryValueChanged(d => d.money)
                .Subscribe(m => moneyText.text = m.ToString("#,##0"));
            dataContext.userData
                .ObserveEveryValueChanged(d => d.hp)
                .Subscribe(m => hpText.text = m.ToString("#,##0"));
        }
    }
}