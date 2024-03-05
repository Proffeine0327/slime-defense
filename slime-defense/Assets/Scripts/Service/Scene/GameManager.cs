using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //service
    private DataContext dataContext => ServiceProvider.Get<DataContext>();
    private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();

    //member
    private bool isGameStarted;
    private bool isGameEnd;
    private bool isWaveStart;
    private HashSet<Enemy> enemies = new();
    private Dictionary<string, Queue<Enemy>> enemyPools = new();

    //property
    private SaveData saveData => dataContext.userData.saveData;
    private int maxWave => dataContext.stageDatas[saveData.stage].waveDatas.Count;

    public bool IsWaveStart => isWaveStart;

    public event Action OnWaveStart;
    public event Action OnWaveEnd;

    //method
    private void Awake()
    {
        ServiceProvider.Register(this);
    }

    private void Start()
    {
        StartCoroutine(GameRoutine());
    }

    public void StartWave()
    {
        isWaveStart = true;
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
                    dataContext.userData.saveData.life -= 1;
                };
                queue.Enqueue(enemy);
                yield return null;
                enemy.gameObject.SetActive(false);
            }
            enemyPools.Add(data.Key, queue);
        }

        isGameStarted = true;

        yield return null;
        while (true)
        {
            Debug.Log(saveData.stage);
            var stageData = dataContext.stageDatas[saveData.stage];
            Debug.Log(maxWave);
            Debug.Log(saveData.wave);
            var waveData = stageData.waveDatas[saveData.wave % maxWave];

            if (!saveData.isInfinity && saveData.wave == maxWave)
            {
                isGameEnd = true;
                Time.timeScale = 0;
                // gameClear.Open();
                yield break;
            }

            yield return new WaitUntil(() => isWaveStart);

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

            dataContext.userData.saveData.money += stageData.gainMoney;
            isWaveStart = false;
            saveData.wave++;

            yield return new WaitForSeconds(1f);

            var nextWaveData = stageData.waveDatas[saveData.wave % maxWave];
            if (nextWaveData.argument) ; //argumentManager.DisplayArgument();

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
            enemylist[i].Appeare((saveData.wave / maxWave) + 1);
            yield return delay;
        }
    }

    private Dictionary<string, int> GetMaxEnemyCount()
    {
        var container = new Dictionary<string, int>();
        foreach (var waveData in dataContext.stageDatas[saveData.wave].waveDatas)
        {
            var waveEnemyMaxCount = new Dictionary<string, int>();

            foreach (var spawnData in waveData.spawnDatas)
            {
                if(!waveEnemyMaxCount.TryAdd(spawnData.key, spawnData.count))
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