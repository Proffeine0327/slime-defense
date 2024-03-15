using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.Services;

namespace Game.GameScene
{
    public partial class Slime : UnitBase, ISelectable, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //services
        private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();
        private GameManager gameManager => ServiceProvider.Get<GameManager>();
        private InputManager inputManager => ServiceProvider.Get<InputManager>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();
        private ObjectPool objectPool => ServiceProvider.Get<ObjectPool>();
        private Grids grids => ServiceProvider.Get<Grids>();

        //field
        private bool isPreview;
        private bool isLook;
        private bool isDragging;
        private string slimeKey;
        private ReactiveProperty<int> lv = new();
        private Vector2Int xy;
        private Animator animator;
        private Attacker attacker;
        private RangeDisplayer rangeDisplayer;

        //property
        private SlimeData SlimeData => dataContext.slimeDatas[slimeKey];
        private bool IsSelected => ReferenceEquals(this, selectManager.CurrentSelect);

        protected override Stats BaseStat => SlimeData.stats[lv.Value - 1];

        public int Lv => lv.Value;
        public Stats DisplayStat => maxStats;
        public Sprite Icon => resourceLoader.slimeIcons.GetValueOrDefault(slimeKey);
        public string SlimeKey => slimeKey;
        public bool IsMaxLv => dataContext.gameData.maxLv == lv.Value;
        public bool IsRemovable => true;
        public float RemoveCost => SlimeData.cost * dataContext.gameData.sellReceiveRatio * Mathf.Pow(2, lv.Value - 1);
        public string RemoveExplain => $"판매: +{RemoveCost}<sprite=\"coin-slime\" name=\"coin-slime\">";
        public SkillBase Skill => skill;

        //method
        protected override void Initialize()
        {
            base.Initialize();

            animator = new(this);
            attacker = new(this);

            rangeDisplayer = GetComponentInChildren<RangeDisplayer>(true);

            curStats.OnStatChanged += (key, pre, cur) =>
            {
                if (key != Stats.Key.AttackRange) return;
                rangeDisplayer.SetRange(curStats.GetStat(key));
            };
            lv.Subscribe(x =>
            {
                if (x <= 0) return;
                modifier.CalculateAll(maxStats, BaseStat);
            });
        }

        private void Update()
        {
            if (isPreview)
            {
                rangeDisplayer.Active(true);
                return;
            }

            rangeDisplayer.Active(IsSelected || isDragging);
            if (!gameManager.IsWaveStart) return;

            if (isLook) LookEnemy(attacker.Target);

            if (curStats.GetStat(Stats.Key.AttackDelay) < maxStats.GetStat(Stats.Key.AttackDelay))
            {
                curStats.ModifyStat(Stats.Key.AttackDelay, x => x + Time.deltaTime);
            }
            else
            {
                if (attacker.HasTarget())
                {
                    isLook = true;
                    animator.PlayAttack(SlimeData.atkAnimKey);
                    curStats.ModifyStat(Stats.Key.AttackDelay, x => 0);
                }
            }
        }

        public void LevelUp()
        {
            lv.Value++;
            Debug.Log(lv.Value);
        }

        public void Attack()
        {
            isLook = false;
            if (!string.IsNullOrEmpty(SlimeData.bulletKey))
            {
                var bullet = objectPool.GetObject(SlimeData.bulletKey);
                bullet.transform.position = transform.position;
                bullet.GetComponent<Bullet>().Fire(attacker.Target, target =>
                {
                    skill.OnAttack(target);
                    objectPool
                        .GetObject
                        (
                            SlimeData.atkParticleKey,
                            attacker.Target.transform.position + Vector3.up * 0.5f
                        )
                        .GetComponent<Particle>()
                        .Play();
                });
            }
            else
            {
                skill.OnAttack(attacker.Target);
                objectPool
                    .GetObject
                    (
                        SlimeData.atkParticleKey,
                        attacker.Target.transform.position + Vector3.up * 0.5f
                    )
                    .GetComponent<Particle>()
                    .Play();
            }
        }

        public void LookEnemy(Enemy enemy)
        {
            var diff = enemy.transform.position - transform.position;
            var rotate = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotate, 0);
        }

        public void MoveTo(Vector2Int to)
        {
            xy = to;
            transform.position = grids.ToPos(to);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            selectManager.Select(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (gameManager.IsWaveStart) return;

            isDragging = true;
            selectManager.Select(null);
            grids.DisplayGrids(SlimeData.grid);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (gameManager.IsWaveStart) return;

            var plane = new Plane(Vector3.down, Vector3.zero);
            if (plane.Raycast(inputManager.TouchRay, out var dist))
            {
                var hitPoint = inputManager.TouchRay.GetPoint(dist);
                if (grids.PosInGrid(hitPoint)) transform.position = grids.Snap(hitPoint);
                else transform.position = hitPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (gameManager.IsWaveStart) return;

            if (grids.PosInGrid(transform.position))
            {
                var movedIndex = grids.ToIndex(transform.position);

                if (slimeManager.MoveSlime(xy, movedIndex)) MoveTo(movedIndex);
                else MoveTo(xy);
            }
            else
            {
                MoveTo(xy);
            }

            isDragging = false;
            grids.HideAllGrids();
            selectManager.Select(this);
        }

        public void OnRemove()
        {
            slimeManager.SellSlime(xy);
        }

        private void OnDestroy()
        {
            slimeManager.Slimes.Remove(this);
        }
    }
}