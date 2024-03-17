using Game.Services;

namespace Game.GameScene
{
    public class EffectBase : ISaveLoad
    {
        //service
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        public UnitBase owner;

        public virtual void OnAdd() { }
        public virtual void OnWaveEnd() { }
        public virtual void OnRemove() { }

        public virtual string Save() { return string.Empty; }
        public virtual void Load(string data) { }

        public EffectBase()
        {
            gameManager.OnWaveEnd += OnWaveEnd;
        }

        ~EffectBase()
        {
            gameManager.OnWaveEnd -= OnWaveEnd;
        }
    }
}