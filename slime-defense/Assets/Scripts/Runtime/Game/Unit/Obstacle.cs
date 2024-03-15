using UnityEngine;
using UniRx;
using Game.Services;
using UnityEngine.EventSystems;

namespace Game.GameScene
{
    public class Obstacle : UnitBase, ISelectable, IPointerClickHandler
    {
        //service
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();

        [SerializeField] private int wave;
        [SerializeField] private int cost;

        private Endurance endurance => skill as Endurance;
        protected override Stats BaseStat => maxStats;

        public int Lv => 1;
        public bool IsRemovable => dataContext.userData.saveData.money >= cost && !endurance.removing;
        public float RemoveCost => -cost;
        public string RemoveExplain => $"제거: {RemoveCost}<sprite=\"coin-slime\" name=\"coin-slime\">";
        public Sprite Icon => null;
        public Stats DisplayStat => maxStats;
        public SkillBase Skill => skill;
        public bool IsSelected => ReferenceEquals(this, selectManager.CurrentSelect);

        private void Start()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
            skill = new Endurance(this);
            endurance.wave.Value = wave;
            endurance.wave.Subscribe(x =>
            {
                if(x > 0) return;
                gameObject.SetActive(false);
            });
        }

        public void OnRemove()
        {
            endurance.removing = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            selectManager.Select(this);
        }
    }
}