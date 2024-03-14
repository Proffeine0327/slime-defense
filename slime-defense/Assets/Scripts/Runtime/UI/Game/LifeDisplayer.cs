using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GameScene
{
    public class LifeDisplayer : MonoBehaviour
    {
        //service
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private Image heartPrefab;

        private List<Image> heart = new();

        private void Start()
        {
            dataContext.userData.saveData
                .ObserveEveryValueChanged(s => s.life)
                .Pairwise()
                .Subscribe(p =>
                {
                    var pre = p.Previous;
                    var cur = p.Current;

                    if(cur < pre) //hp down
                    {

                    }
                    else
                    {
                        
                    }
                });
        }
    }
}