using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GameScene
{
    [ExecuteAlways]
    public class LifeContainer : MonoBehaviour
    {
        //service
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private LifeDisplayer lifePrefab;
        [SerializeField] private Vector2 offset;
        [SerializeField] private float dist;

        private List<LifeDisplayer> lifes = new();

        private void Start()
        {
            dataContext.userData.saveData
                .ObserveEveryValueChanged(s => s.life)
                .Subscribe(l =>
                {
                    int i = 1;
                    while (l - 1 >= lifes.Count) lifes.Add(Instantiate(lifePrefab, transform));

                    for (; i <= l; i++) lifes[i - 1].IsDisplay.Value = true;
                    for (; i <= lifes.Count; i++) lifes[i - 1].IsDisplay.Value = false;
                });
        }

        private void Update()
        {
            var count = 0;
            foreach (RectTransform life in transform)
                life.anchoredPosition = offset + Vector2.right * dist * count++;
        }
    }
}