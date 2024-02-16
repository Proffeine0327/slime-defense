using System;
using System.Collections.Generic;
using UnityEngine;

public partial class DataContext
{
    [Serializable]
    public class SlimeData
    {
        public string name;
        public string modelkey;
        public string atkAnimKey;
        public string atkParticleKey;
        public Stat[] stats;

        public static SlimeData Parse(string csv)
        {
            var data = new SlimeData();
            var split = csv.Split(',');
            data.name = split[0];
            data.modelkey = split[1];
            data.atkAnimKey = split[2];
            data.atkParticleKey = split[3];

            var stats = new List<Stat>();
            var maxLv = ServiceProvider.Get<DataContext>().gameData.maxLv;
            for (int i = 0; i <= maxLv; i++)
            {
                var stat = new Stat();
                stat.AddStat("attack range", float.Parse(split[4]));
                stat.AddStat("attack damage", Mathf.Lerp(i / (float)maxLv, float.Parse(split[5]), float.Parse(split[6])));
                stat.AddStat("ability power", Mathf.Lerp(i / (float)maxLv, float.Parse(split[7]), float.Parse(split[8])));
                stat.AddStat("attack delay", Mathf.Lerp(i / (float)maxLv, float.Parse(split[9]), float.Parse(split[10])));
                stats.Add(stat);
            }
            data.stats = stats.ToArray();

            return data;
        }
    }

    [Serializable]
    public class UserData
    {
        public int hp = 999;
        public int money;
        public string[] deck;
        public bool[] unlockStages;
        public SaveData saveData;
    }

    [Serializable]
    public class GameData
    {
        public int maxLv;
    }

    [Serializable]
    public class SaveData
    {
        public bool isInfinity;
        public int stage = -1;
        public int wave;
        public int money;
        public int life;
        public int killAmount;
        public float playTime;
        public string[] deck;
        public string[] arguments;
        public string[] playerunits;
        public string[] obstacles;
        public string[] grids;
    }
}