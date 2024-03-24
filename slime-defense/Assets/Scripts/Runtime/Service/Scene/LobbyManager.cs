using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Game.Services
{
    public class LobbyManager : MonoBehaviour
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private SoundManager soundManager => ServiceProvider.Get<SoundManager>();

        //const
        private float INTERVAL_TIME = 0.5f;

        //member
        public ReactiveIntervalProperty<int> Stage;
        public ReactiveIntervalProperty<bool> IsSelectedStage;
        public StageData StageData => dataContext.stageDatas[Stage.Value];

        private void Awake()
        {
            ServiceProvider.Register(this);

            Stage = new(this) { Interval = INTERVAL_TIME, Value = 1 };
            IsSelectedStage = new(this) { Interval = INTERVAL_TIME, Value = true };
        }

        private void Start()
        {
            // soundManager.Play("Airship", looping: true);
        }
    }
}