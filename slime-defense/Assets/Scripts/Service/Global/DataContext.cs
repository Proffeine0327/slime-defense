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
        [NonSerialized] public GameData gameData = new();
        [NonSerialized] public Dictionary<string, SlimeData> slimeDatas = new();
        [NonSerialized] public Dictionary<string, EnemyData> enemyDatas = new();
        [NonSerialized] public Dictionary<Tier, TierData> tierDatas = new();
        [NonSerialized] public List<StageData> stageDatas = new();

        //user data
        [NonSerialized] public UserData userData = new();

        private void Start()
        {
            ServiceProvider.Register(this, true);

            tierDatas.Add(Tier.Normal, new TierData { color = new Color32(77, 175, 255, 255) });
            tierDatas.Add(Tier.Epic, new TierData { color = new Color32(184, 77, 255, 255) });
            tierDatas.Add(Tier.Legendary, new TierData { color = new Color32(255, 213, 77, 255) });

            //gameData
            taskWaiter.AddTask(new LoadingTaskData(() =>
            {
                return TSVTask
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
                return TSVTask
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
                return TSVTask
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

            //stage1
            taskWaiter.AddTask(new LoadingTaskData(() =>
            {
                return TSVTask
                (
                    "17ozRp_joGxGO9SQF1tYLCSxHZ7WKmdiTtK16uDKNyvI",
                    1127072629,
                    csv => stageDatas.Add(StageData.Parse(csv))
                );
            }));
            //stage2
            taskWaiter.AddTask(new LoadingTaskData(() =>
            {
                return TSVTask
                (
                    "17ozRp_joGxGO9SQF1tYLCSxHZ7WKmdiTtK16uDKNyvI",
                    577993109,
                    csv => stageDatas.Add(StageData.Parse(csv))
                );
            }));

            taskWaiter.AddTask(new LoadingTaskData(() =>
            {
                userData = UserData.Load();
                return UniTask.DelayFrame(0);
            }));
        }

        private async UniTask TSVTask(string address, long gid, Action<string> action)
        {
            var request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{address}/export?format=tsv&gid={gid}");
            await request.SendWebRequest();
            action?.Invoke(request.downloadHandler.text);
        }

        private async UniTask TSVTask(string address, string range, long gid, Action<string> action)
        {
            var request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{address}/export?format=tsv&range={range}&gid={gid}");
            await request.SendWebRequest();
            action?.Invoke(request.downloadHandler.text);
        }
    }
}