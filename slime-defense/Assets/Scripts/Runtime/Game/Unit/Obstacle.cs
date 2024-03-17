using UnityEngine;
using UniRx;
using Game.Services;
using UnityEngine.EventSystems;
using System.Text;

namespace Game.GameScene
{
    public class Obstacle : UnitBase, ISelectable, ISaveLoad, IPointerClickHandler
    {
        //service
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();

        [SerializeField] private int wave;
        [SerializeField] private int cost;

        private Endurance endurance => skill as Endurance;

        protected override Stats BaseStat => maxStats;

        public int Lv => 1;
        public bool IsRemovable => gameManager.SaveData.money >= cost && !endurance.removing;
        public float RemoveCost => -cost;
        public string RemoveExplain => $"제거: {RemoveCost}<sprite=\"coin-slime\" name=\"coin-slime\">";
        public Sprite Icon => null;
        public Stats DisplayStat => maxStats;
        public SkillBase Skill => skill;
        public bool IsSelected => ReferenceEquals(this, selectManager.CurrentSelect);

        public override void Initialize()
        {
            base.Initialize();
            skill = new Endurance(this);
            endurance.wave.Value = wave;
            endurance.wave.Subscribe(x =>
            {
                if (x > 0) return;
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

        public string Save()
        {
            var sb = new StringBuilder();
            sb.Append(endurance.wave.Value);
            sb.Append(',');
            sb.Append(endurance.removing);
            return sb.ToString();
        }

        public void Load(string data)
        {
            Initialize();
            var split = data.Split(',');
            endurance.wave.Value = int.Parse(split[0]);
            endurance.removing = bool.Parse(split[1]);
        }
    }
}