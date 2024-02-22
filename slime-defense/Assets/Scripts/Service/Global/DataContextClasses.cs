using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlimeData
{
    public string slimeKey;
    public string name;
    public Tier tier;
    public string atkAnimKey;
    public string atkParticleKey;
    public int cost;
    public Stats[] stats;

    public static SlimeData Parse(string csv)
    {
        var data = new SlimeData();
        var split = csv.Split(',');
        data.slimeKey = split[0];
        data.name = split[1];
        data.tier = (Tier)Enum.Parse(typeof(Tier), split[2]);
        data.atkAnimKey = split[3];
        data.atkParticleKey = split[4];
        data.cost = int.Parse(split[5]);

        var stats = new List<Stats>();
        var maxLv = ServiceProvider.Get<DataContext>().gameData.maxLv;
        for (int i = 0; i <= maxLv; i++)
        {
            var stat = new Stats();
            stat.AddStat(Stats.Key.AttackRange, float.Parse(split[6]));
            stat.AddStat(Stats.Key.AttackDamage, Mathf.Lerp(i / (float)maxLv, float.Parse(split[7]), float.Parse(split[8])));
            stat.AddStat(Stats.Key.AbilityPower, Mathf.Lerp(i / (float)maxLv, float.Parse(split[9]), float.Parse(split[10])));
            stat.AddStat(Stats.Key.AttackDelay, Mathf.Lerp(i / (float)maxLv, float.Parse(split[11]), float.Parse(split[12])));
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

[Serializable]
public class TierData
{
    public Color color;
}
public enum Tier { Normal, Epic, Legendary }