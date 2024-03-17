using Game.GameScene;
using UnityEngine;

namespace Game.Services
{
    public class ObstacleManager : MonoBehaviour, ISaveLoad, IInitialize
    {
        [SerializeField] private Obstacle[] obstacles;

        public void Load(string data)
        {
            var obstacles = JsonUtility.FromJson<StringListWrapper>(data);
            var count = 0;
            foreach (var o in this.obstacles)
                o.Load(count >= obstacles.datas.Count ? null : obstacles.datas[count++]);
        }
        
        public void Initialize()
        {
            foreach(var o in obstacles)
                o.Initialize();
        }

        public string Save()
        {
            var obstacles = new StringListWrapper();
            foreach (var o in this.obstacles)
                obstacles.datas.Add(o.Save());
            return JsonUtility.ToJson(obstacles);
        }

        private void Awake()
        {
            ServiceProvider.Register(this);
        }
    }
}