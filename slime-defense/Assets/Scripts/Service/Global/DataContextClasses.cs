using System;
using System.Collections.Generic;
using UnityEngine;
using Game.GameScene;

namespace Game.Services
{
    #region Load
    public class SlimeData
    {
        public static SlimeData Parse(string row)
        {
            var count = 0;
            var data = new SlimeData();
            var split = row.Split(',');
            data.slimeKey = split[count++];
            data.name = split[count++];
            data.tier = Enum.Parse<Tier>(split[count++]);
            data.grid = Enum.Parse<GridType>(split[count++]);
            data.atkAnimKey = split[count++];
            data.bulletKey = split[count++];
            data.atkParticleKey = split[count++];
            data.skillKey = split[count++];
            data.cost = int.Parse(split[count++]);

            var stats = new List<Stats>();
            var sCount = count;
            var maxLv = ServiceProvider.Get<DataContext>().gameData.maxLv;
            for (int i = 0; i < maxLv; i++)
            {
                count = sCount;
                var stat = new Stats();
                stat.AddStat(Stats.Key.AttackRange, float.Parse(split[count++]));
                stat.AddStat(Stats.Key.AttackDamage, Mathf.Lerp(float.Parse(split[count++]), float.Parse(split[count++]), i / (float)maxLv));
                stat.AddStat(Stats.Key.AbilityPower, Mathf.Lerp(float.Parse(split[count++]), float.Parse(split[count++]), i / (float)maxLv));
                stat.AddStat(Stats.Key.AttackDelay, Mathf.Lerp(float.Parse(split[count++]), float.Parse(split[count++]), i / (float)maxLv));
                stats.Add(stat);
                Debug.Log(stat.ToString());
            }
            data.stats = stats.ToArray();

            return data;
        }

        public string slimeKey;
        public string name;
        public Tier tier;
        public GridType grid;
        public string atkAnimKey;
        public string bulletKey;
        public string atkParticleKey;
        public string skillKey;
        public int cost;
        public Stats[] stats;
    }
    public class EnemyData
    {
        public static EnemyData Parse(string row)
        {
            var count = 0;
            var split = row.Split(',');
            var data = new EnemyData();
            data.key = split[count++];
            data.name = split[count++];
            data.baseStat.AddStat(Stats.Key.Hp, float.Parse(split[count++]));
            data.baseStat.AddStat(Stats.Key.Speed, float.Parse(split[count++]));
            data.baseStat.AddStat(Stats.Key.AbilityPower, float.Parse(split[count++]));
            data.upgradeHpPercentage = float.Parse(split[count++]);
            data.upgradeHpAdd = float.Parse(split[count++]);
            data.upgradeSpeedPercentage = float.Parse(split[count++]);
            data.upgradeSpeedAdd = float.Parse(split[count++]);
            data.upgradeApPercentage = float.Parse(split[count++]);
            data.upgradeApAdd = float.Parse(split[count++]);
            return data;
        }

        public string key;
        public string name;
        public Stats baseStat = new();
        public float upgradeHpPercentage;
        public float upgradeHpAdd;
        public float upgradeSpeedPercentage;
        public float upgradeSpeedAdd;
        public float upgradeApPercentage;
        public float upgradeApAdd;
    }
    public class GameData
    {
        public int maxLv;
    }
    public class TierData
    {
        public Color color;
    }
    public enum Tier { Normal, Epic, Legendary }
    public class StageData
    {
        public static StageData Parse(string csv)
        {
            var split = csv.Split('\n');
            var stageRows = split[1].Split(',');
            var newStageData = new StageData
            {
                name = stageRows[0],
                explain = stageRows[1],
                startMoney = int.Parse(stageRows[2]),
                gainMoney = int.Parse(stageRows[3])
            };

            var waveRows = split[4..];
            WaveData newWaveData = default;
            foreach (var row in waveRows)
            {
                var rowSplit = row.Split(',');
                if (rowSplit[0] != string.Empty)
                {
                    if (newWaveData != default)
                        newStageData.waveDatas.Add(newWaveData);
                    newWaveData = new();
                    newWaveData.gainMoney = int.Parse(rowSplit[1]);
                    newWaveData.argument = rowSplit[2] != string.Empty;
                }
                var enemySpawnData = new EnemySpawnData();
                enemySpawnData.key = rowSplit[3];
                enemySpawnData.wave = int.Parse(rowSplit[4]);
                enemySpawnData.wait = float.Parse(rowSplit[5]);
                enemySpawnData.delay = float.Parse(rowSplit[6]);
                enemySpawnData.count = int.Parse(rowSplit[7]);
                newWaveData.spawnDatas.Add(enemySpawnData);
            }
            newStageData.waveDatas.Add(newWaveData);
            return newStageData;
        }

        public string name;
        public string explain;
        public int startMoney;
        public int gainMoney;
        public List<WaveData> waveDatas = new();

        public class WaveData
        {
            public int gainMoney;
            public bool argument;
            public List<EnemySpawnData> spawnDatas = new();
        }

        public class EnemySpawnData
        {
            public int wave;
            public string key;
            public int path;
            public float wait;
            public float delay;
            public int count;
        }
    }
    #endregion

    #region Save/Load
    [Serializable]
    public class UserData
    {
        public int hp = 999;
        public int money;
        public string[] deck;
        public bool[] unlockStages;
        public SaveData saveData = new();
    }

    [Serializable]
    public class SaveData
    {
        public bool isInfinity;
        public int stage;
        public int wave;
        public int money = 10000;
        public int life;
        public int killAmount;
        public float playTime;
        public string[] deck;
        public string[] arguments;
        public string[] playerunits;
        public string[] obstacles;
        public string[] grids;
    }
    #endregion
}