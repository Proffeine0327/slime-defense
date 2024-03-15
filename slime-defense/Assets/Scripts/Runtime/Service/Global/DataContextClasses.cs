using System;
using System.Collections.Generic;
using UnityEngine;
using Game.GameScene;
using System.Linq;

namespace Game.Services
{
    #region Load
    public class SlimeData
    {
        public static SlimeData Parse(string row)
        {
            var count = 0;
            var data = new SlimeData();
            var split = row.Split('\t');
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
        public string key;
        public string name;
        public string explain;
        public string skillKey;
        public Stats @base = new();
        public Stats percentage = new();
        public Stats add = new();

        public static EnemyData Parse(string row)
        {
            var count = 0;
            var split = row.Split('\t');
            var data = new EnemyData();
            data.key = split[count++];
            data.name = split[count++];
            data.explain = split[count++];
            data.skillKey = split[count++];
            data.@base.AddStat(Stats.Key.Hp, float.Parse(split[count++]));
            data.@base.AddStat(Stats.Key.Speed, float.Parse(split[count++]));
            data.@base.AddStat(Stats.Key.AbilityPower, float.Parse(split[count++]));
            data.percentage.AddStat(Stats.Key.Hp, float.Parse(split[count++]));
            data.add.AddStat(Stats.Key.Hp, float.Parse(split[count++]));
            data.percentage.AddStat(Stats.Key.Speed, float.Parse(split[count++]));
            data.add.AddStat(Stats.Key.Speed, float.Parse(split[count++]));
            data.percentage.AddStat(Stats.Key.AbilityPower, float.Parse(split[count++]));
            data.add.AddStat(Stats.Key.AbilityPower, float.Parse(split[count++]));
            return data;
        }
    }
    public class GameData
    {
        public int maxLv;
        public float sellReceiveRatio;

        public static GameData Parse(string tsv)
        {
            var split = tsv.Split('\t');
            var data = new GameData();
            data.maxLv = int.Parse(split[0]);
            data.sellReceiveRatio = float.Parse(split[1]);

            return data;
        }
    }
    public class TierData
    {
        public Color color;
    }
    public enum Tier { Normal, Epic, Legendary }
    public class StageData
    {
        public static StageData Parse(string tsv)
        {
            var split = tsv.Split('\n');
            var stageRows = split[1].Split('\t');
            var newStageData = new StageData();
            newStageData.name = stageRows[0];
            newStageData.explain = stageRows[1];
            newStageData.unlockMoney = int.Parse(stageRows[2]);
            newStageData.startLife = int.Parse(stageRows[3]);
            newStageData.startMoney = int.Parse(stageRows[4]);
            newStageData.gainMoney = int.Parse(stageRows[5]);

            var waveRows = split[4..];
            WaveData newWaveData = default;
            foreach (var row in waveRows)
            {
                var rowSplit = row.Split('\t');
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
        public int unlockMoney;
        public int startLife;
        public int startMoney;
        public int gainMoney;
        public List<WaveData> waveDatas = new();

        public string[] AllAppeareEnemies
        {
            get
            {
                var list = new HashSet<string>();
                foreach (var waveData in waveDatas)
                    foreach (var spawnData in waveData.spawnDatas)
                        list.Add(spawnData.key);
                return list.ToArray();
            }
        }

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
        public float money;
        public List<string> deck = new() { "GrassSlime", "DevilSlime", "AngleSlime", "IceSlime", "LavaSlime", "MetalSlime", };
        public bool[] unlockStages = new[] { true };
        public bool[] unlockInfModes = new[] { false };
        public SaveData saveData = new();

        public void Save()
        {
            var json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString("userdata", json);
            PlayerPrefs.Save();
        }

        public static UserData Load()
        {
            var dataContext = ServiceProvider.Get<DataContext>();

            var json = PlayerPrefs.GetString("userdata");
            var userdata = JsonUtility.FromJson<UserData>(json) ?? new UserData();

            var adjUnlockStages = Enumerable.Repeat(false, dataContext.stageDatas.Count).ToArray();
            if (userdata.unlockStages != null)
                Array.Copy(userdata.unlockStages, adjUnlockStages, Mathf.Min(userdata.unlockStages.Length, adjUnlockStages.Length));
            userdata.unlockStages = adjUnlockStages;

            var adjUnlockInfModes = Enumerable.Repeat(false, dataContext.stageDatas.Count).ToArray();
            if (userdata.unlockInfModes != null)
                Array.Copy(userdata.unlockInfModes, adjUnlockInfModes, Mathf.Min(userdata.unlockInfModes.Length, adjUnlockInfModes.Length));
            userdata.unlockInfModes = adjUnlockInfModes;

            userdata.money = 10000;

            return userdata;
        }

        public void CreateNewSaveData(int stage, bool isInfinity)
        {
            var dataContext = ServiceProvider.Get<DataContext>();

            saveData = new SaveData()
            {
                stage = stage,
                isInfinity = isInfinity,
                deck = dataContext.userData.deck.ToArray(),
                money = dataContext.stageDatas[stage].startMoney,
                life = dataContext.stageDatas[stage].startLife
            };
        }
    }

    [Serializable]
    public class SaveData
    {
        public int stage;
        public bool isInfinity;
        public int wave = 1;
        public int life;
        public int killAmount;
        public float money = 10000;
        public float playTime;
        public string[] deck;
        public string[] arguments;
        public string[] playerunits;
        public string[] obstacles;
    }
    #endregion
}