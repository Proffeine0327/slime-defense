using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public partial class DataContext : MonoBehaviour
{
    //csv data
    public GameData gameData = new();
    public Dictionary<string, SlimeData> slimeDatas = new();

    //user data
    public UserData userData = new();

    private async void Awake()
    {
        ServiceProvider.Register(this, true);

        await CSVTask
        (
            "1LMTnNi6RDe2d1KetKZzUSslsx3iuZzvmK8upQAx8Xw8",
            "A2",
            0,
            csv => gameData.maxLv = int.Parse(csv)
        );

        await CSVTask
        (
            "100EntXVK5z7Ms334Ay-XTuyL0F7P7oo-q0-vHn2YGPs",
            "A:K",
            0,
            csv =>
            {
                var split = csv.Split('\n');
                foreach(var s in split)
                {
                    var data = SlimeData.Parse(s);
                    slimeDatas.Add(data.name, data);
                }
            }
        );
    }

    private async UniTask CSVTask(string address, string range, long gid, Action<string> action)
    {
        var request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{address}/export?format=csv&range={range}&gid={gid}");
        await request.SendWebRequest();
        action?.Invoke(request.downloadHandler.text);
    }
}