using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public partial class DataContext : MonoBehaviour
{
    //services
    private TaskWaiter taskWaiter => ServiceProvider.Get<TaskWaiter>();

    //csv data
    public GameData gameData = new();
    public Dictionary<string, SlimeData> slimeDatas = new();
    public Dictionary<Tier, TierData> tierDatas = new();

    //user data
    public UserData userData = new();

    private void Start()
    {
        ServiceProvider.Register(this, true);

        tierDatas.Add(Tier.Normal, new TierData { color = new Color32(77, 175, 255, 255) });
        tierDatas.Add(Tier.Epic, new TierData { color = new Color32(184, 77, 255, 255) });
        tierDatas.Add(Tier.Legendary, new TierData { color = new Color32(255, 213, 77, 255) });

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

        taskWaiter.AddTask(new LoadingTaskData(() =>
        {
            return CSVTask
            (
                "100EntXVK5z7Ms334Ay-XTuyL0F7P7oo-q0-vHn2YGPs",
                "A2:M",
                0,
                csv =>
                {
                    var split = csv.Split('\n');
                    foreach (var s in split)
                    {
                        var data = SlimeData.Parse(s);
                        slimeDatas.Add(data.slimeKey, data);
                    }
                }
            );
        }));
    }

    private async UniTask CSVTask(string address, string range, long gid, Action<string> action)
    {
        var request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{address}/export?format=csv&range={range}&gid={gid}");
        await request.SendWebRequest();
        action?.Invoke(request.downloadHandler.text);
    }
}