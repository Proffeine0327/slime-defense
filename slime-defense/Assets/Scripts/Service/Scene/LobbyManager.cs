using UnityEngine;

namespace Game.Services
{
    public class LobbyManager : MonoBehaviour
    {
        //Services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        private int stage;

        public int Stage
        {
            get => stage;
            set
            {
                stage = Mathf.Clamp(value, 0, dataContext.stageDatas.Count);
            }
        }
        public bool IsSelectedStage { get; set; }
        public StageData StageData => dataContext.stageDatas[Stage];

        private void Awake()
        {
            ServiceProvider.Register(this);

            IsSelectedStage = true;
        }
    }
}