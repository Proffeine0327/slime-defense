using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.GameScene;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

namespace Game.Services
{
    public class GameManager : MonoBehaviour
    {
        //service
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private ArgumentManager argumentManager => ServiceProvider.Get<ArgumentManager>();
        private ScreenFade screenFade => ServiceProvider.Get<ScreenFade>();
        private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();
        private ObstacleManager obstacleManager => ServiceProvider.Get<ObstacleManager>();

        //member
        [SerializeField] private Obstacle[] obstacles;

        private SaveData saveData;
        private bool isWaveStarted;
        private bool isGameClear;
        private bool isGameOver;
        private HashSet<Enemy> enemies = new();
        private Dictionary<string, Queue<Enemy>> enemyPools = new();

        //property
        public SaveData SaveData => saveData;
        public bool IsWaveStart => isWaveStarted;
        public bool IsGameClear => isGameClear;
        public bool IsGameOver => isGameOver;
        public int MaxWave => dataContext.stageDatas[saveData.stage].waveDatas.Count;
        public HashSet<Enemy> Enemies => enemies;

        public event Action OnWaveStart;
        public event Action OnWaveEnd;

        //method
        private void Awake()
        {
            ServiceProvider.Register(this);
            saveData = dataContext.userData.saveData.Clone();
        }

        private void Start()
        {
            if (saveData.isNewGame)
            {
                saveData.isNewGame = false;
                argumentManager.Initialize();
                slimeManager.Initialize();
                obstacleManager.Initialize();
            }
            else
            {
                argumentManager.Load(saveData.arguments);
                slimeManager.Load(saveData.slimes);
                obstacleManager.Load(saveData.obstacles);
            }
            StartCoroutine(GameRoutine());
        }
        
        public void RemoveGame()
        {
            dataContext.userData.saveData = null;
            ExitGame();
        }

        public void TrySaveGame()
        {
            if(isWaveStarted) return;

            SaveGame();
        }

        private void SaveGame()
        {
            saveData.arguments = argumentManager.Save();
            saveData.slimes = slimeManager.Save();
            saveData.obstacles = obstacleManager.Save();
            dataContext.userData.Save();

            dataContext.userData.saveData = saveData.Clone();
        }

        public void ExitGame()
        {
            screenFade
                .Fade()
                .LoadScene(async () => await SceneManager.LoadSceneAsync("Lobby"));
        }

        public void StartWave()
        {
            isWaveStarted = true;
        }

        private void Update()
        {
            if (saveData.life <= 0)
            {
                Time.timeScale = 0;
                isGameOver = true;
            }
        }

        private IEnumerator GameRoutine()
        {
            foreach (var data in GetMaxEnemyCount())
            {
                var queue = new Queue<Enemy>();
                for (int i = 0; i < data.Value; i++)
                {
                    var key = data.Key;
                    var enemy = new Enemy.Builder(key).Build();
                    enemy.transform.SetParent(transform);
                    enemy.OnDeath += () =>
                    {
                        enemyPools[key].Enqueue(enemy);
                        enemies.Remove(enemy);

                    };
                    enemy.OnArrive += () =>
                    {
                        enemyPools[key].Enqueue(enemy);
                        enemies.Remove(enemy);
                    };
                    queue.Enqueue(enemy);
                    yield return null;
                    enemy.gameObject.SetActive(false);
                }
                enemyPools.Add(data.Key, queue);
            }

            // isGameStarted = true;

            yield return null;
            while (true)
            {
                var stageData = dataContext.stageDatas[saveData.stage];
                var waveData = stageData.waveDatas[(saveData.wave - 1) % MaxWave];

                if (!saveData.isInfinity && (saveData.wave - 1) == MaxWave)
                {
                    isGameClear = true;
                    Time.timeScale = 0;
                    // gameClear.Open();
                    yield break;
                }

                yield return new WaitUntil(() => isWaveStarted);
                SaveGame();

                enemies.Clear();
                Debug.Log("Clear");
                foreach (var data in waveData.spawnDatas)
                {
                    Debug.Log(data.key);
                    StartCoroutine(SpawnEnemy(data));
                }
                yield return null;
                Debug.Log(enemies.Count);
                OnWaveStart?.Invoke();

                yield return new WaitUntil(() => enemies.Count == 0);

                saveData.money += stageData.gainMoney;
                isWaveStarted = false;
                saveData.wave++;

                yield return new WaitForSeconds(1f);

                var nextWaveData = stageData.waveDatas[(saveData.wave - 1) % MaxWave];
                if (nextWaveData.argument)
                {
                    argumentManager.DisplayArgument();
                    yield return new WaitUntil(() => argumentManager.IsSelected);
                }

                OnWaveEnd?.Invoke();
            }
        }

        private IEnumerator SpawnEnemy(StageData.EnemySpawnData data)
        {
            var delay = new WaitForSeconds(data.wait);

            var enemylist = new Enemy[data.count];
            for (int i = 0; i < data.count; i++)
            {
                var enemy = enemyPools[data.key].Dequeue();
                enemylist[i] = enemy;
                enemies.Add(enemy);
            }

            yield return new WaitForSeconds(data.delay);

            for (int i = 0; i < data.count; i++)
            {
                enemylist[i].gameObject.SetActive(true);
                enemylist[i].Appeare(((saveData.wave - 1) / MaxWave) + 1);
                yield return delay;
            }
        }

        private Dictionary<string, int> GetMaxEnemyCount()
        {
            var container = new Dictionary<string, int>();
            foreach (var waveData in dataContext.stageDatas[saveData.stage].waveDatas)
            {
                var waveEnemyMaxCount = new Dictionary<string, int>();

                foreach (var spawnData in waveData.spawnDatas)
                {
                    if (!waveEnemyMaxCount.TryAdd(spawnData.key, spawnData.count))
                        waveEnemyMaxCount[spawnData.key] += spawnData.count;
                }

                foreach (var spawnData in waveEnemyMaxCount)
                {
                    if (!container.ContainsKey(spawnData.Key))
                        container.Add(spawnData.Key, spawnData.Value);
                    else
                    {
                        if (container[spawnData.Key] < spawnData.Value)
                            container[spawnData.Key] = spawnData.Value;
                    }
                }
            }
            return container;
        }
    }
}