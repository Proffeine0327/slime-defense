using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Services
{
    public partial class DataContext : MonoBehaviour
    {
        //services
        private TaskWaiter taskWaiter => ServiceProvider.Get<TaskWaiter>();

        //csv data
        public GameData gameData = new();
        public Dictionary<string, SlimeData> slimeDatas = new();
        public Dictionary<string, EnemyData> enemyDatas = new();
        public Dictionary<Tier, TierData> tierDatas = new();
        public List<StageData> stageDatas = new();

        //user data
        public UserData userData = new();

        private void Start()
        {
            ServiceProvider.Register(this, true);

            userData.saveData.money = 10000;

            tierDatas.Add(Tier.Normal, new TierData { color = new Color32(77, 175, 255, 255) });
            tierDatas.Add(Tier.Epic, new TierData { color = new Color32(184, 77, 255, 255) });
            tierDatas.Add(Tier.Legendary, new TierData { color = new Color32(255, 213, 77, 255) });

            //gameData
            taskWaiter.AddTask(new LoadingTaskData(() =>
            {
                return CSVTask
                (
                    "1LMTnNi6RDe2d1KetKZzUSslsx3iuZzvmK8upQAx8Xw8",
                    "A2",
                    0,
                    csv => gameData.maxLv = int.Parse(csv)
                );
            }));

            //slimeData
            taskWaiter.AddTask(new LoadingTaskData(() =>
            {
                return CSVTask
                (
                    "100EntXVK5z7Ms334Ay-XTuyL0F7P7oo-q0-vHn2YGPs",
                    "2:100",
                    0,
                    csv =>
                    {
                        var split = csv.Split('\n');
                        foreach (var row in split)
                        {
                            var data = SlimeData.Parse(row);
                            slimeDatas.Add(data.slimeKey, data);
                        }
                    }
                );
            }));

            //enemyData
            taskWaiter.AddTask(new LoadingTaskData(() =>
            {
                return CSVTask
                (
                    "19Qvrg781qdkjG6Ud-DcEdvpx4hM_hwrb67n1U574g9Y",
                    "2:100",
                    0,
                    csv =>
                    {
                        var split = csv.Split('\n');
                        foreach (var row in split)
                        {
                            var data = EnemyData.Parse(row);
                            enemyDatas.Add(data.key, data);
                        }
                    }
                );
            }));

            //stageData
            taskWaiter.AddTask(new LoadingTaskData(() =>
            {
                return CSVTask
                (
                    "17ozRp_joGxGO9SQF1tYLCSxHZ7WKmdiTtK16uDKNyvI",
                    1127072629,
                    csv =>
                    {
                        stageDatas.Add(StageData.Parse(csv));
                    }
                );
            }));
        }

        private async UniTask CSVTask(string address, long gid, Action<string> action)
        {
            var request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{address}/export?format=csv&gid={gid}");
            await request.SendWebRequest();
            action?.Invoke(request.downloadHandler.text);
        }

        private async UniTask CSVTask(string address, string range, long gid, Action<string> action)
        {
            var request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{address}/export?format=csv&range={range}&gid={gid}");
            await request.SendWebRequest();
            action?.Invoke(request.downloadHandler.text);
        }
    }
}