using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MekaruStudios.MonsterCreator;
using UniRx;
using UnityEngine;

namespace Game.GameScene
{
    public partial class Stats
    {
        public class Modifier : ISaveLoad
        {
            public class Info
            {
                public ReactiveProperty<float> add = new();
                public ReactiveProperty<float> percent = new();

                public Info(float add, float percent)
                {
                    this.add.Value = add;
                    this.percent.Value = percent;
                }
            }

            private ReactiveDictionary<string, ReactiveDictionary<Key, Info>> casterInfo = new();
            private Stats percentValues = new();
            private Stats addValues = new();

            public event Action<Key> OnValueChange;

            public Modifier()
            {
                //add
                casterInfo
                    .ObserveAdd()
                    .Subscribe(kvp =>
                    {
                        kvp.Value
                            .ObserveAdd()
                            .Subscribe(e =>
                            {
                                var key = e.Key;
                                var percent = e.Value.percent;
                                var add = e.Value.add;

                                percent
                                    .Pairwise()
                                    .Subscribe(p =>
                                    {
                                        var oldValue = p.Previous;
                                        var newValue = p.Current;
                                        percentValues.ModifyStat(key, x => x + (newValue - oldValue));
                                        Debug.Log(percentValues.ToString());
                                    });
                                add
                                    .Pairwise()
                                    .Subscribe(p =>
                                    {
                                        var oldValue = p.Previous;
                                        var newValue = p.Current;
                                        addValues.ModifyStat(key, x => x + (newValue - oldValue));
                                        Debug.Log(addValues.ToString());
                                    });
                                percentValues.ModifyStat(key, x => x + percent.Value);
                                addValues.ModifyStat(key, x => x + add.Value);
                            });

                        //remove
                        kvp.Value
                            .ObserveRemove()
                            .Subscribe(e =>
                            {
                                var key = e.Key;
                                var percent = e.Value.percent.Value;
                                var add = e.Value.add.Value;

                                percentValues.ModifyStat(key, x => x - percent);
                                addValues.ModifyStat(key, x => x - add);

                                if (casterInfo[kvp.Key].Count == 0)
                                    casterInfo.Remove(kvp.Key);
                            });

                        //replace
                        kvp.Value
                            .ObserveReplace()
                            .Subscribe(e =>
                            {
                                var key = e.Key;
                                var oldPercent = e.OldValue.percent.Value;
                                var oldAdd = e.OldValue.add.Value;
                                var newPercent = e.NewValue.percent.Value;
                                var newAdd = e.NewValue.add.Value;

                                percentValues.ModifyStat(key, x => x + (newPercent - oldPercent));
                                addValues.ModifyStat(key, x => x + (newAdd - oldAdd));
                            });
                    });

                casterInfo
                    .ObserveRemove()
                    .Subscribe(kvp =>
                    {
                        foreach (var info in kvp.Value)
                        {
                            percentValues.ModifyStat(info.Key, x => x - info.Value.percent.Value);
                            addValues.ModifyStat(info.Key, x => x - info.Value.add.Value);
                        }
                    });

                percentValues.OnStatChanged += (key, _, _) => OnValueChange?.Invoke(key);
                addValues.OnStatChanged += (key, _, _) => OnValueChange?.Invoke(key);
            }

            /// <summary>
            /// Add/Change modify infomation
            /// if set both value(add, percent) 0, remove
            /// </summary>
            public void Set
            (
                string caster,
                Key key,
                Func<float, float> percent,
                Func<float, float> add
            )
            {
                if (!casterInfo.ContainsKey(caster))
                {
                    if (add(0) == 0 && percent(0) == 0) return;
                    casterInfo.Add(caster, new());
                    casterInfo[caster].Add(key, new(add(0), percent(0)));
                    Debug.Log($"{add(0)} {percent(0)}");
                    return;
                }

                if (!casterInfo[caster].ContainsKey(key))
                {
                    if (add(0) == 0 && percent(0) == 0) return;
                    casterInfo[caster].Add(key, new(add(0), percent(0)));
                    Debug.Log($"{add(0)} {percent(0)}");
                    return;
                }

                var info = casterInfo[caster][key];
                if (add(info.add.Value) == 0 && percent(info.percent.Value) == 0)
                {
                    casterInfo[caster].Remove(key);
                    return;
                }

                info.add.Value = add(info.add.Value);
                info.percent.Value = percent(info.percent.Value);
                Debug.Log($"{info.add.Value} {info.percent.Value}");
            }

            public IReadOnlyDictionary<Key, float> GetPercentValues()
            {
                return percentValues.GetStats();
            }

            public IReadOnlyDictionary<Key, float> GetAddValues()
            {
                return addValues.GetStats();
            }

            public void CalculateAll(Stats targetStats, Stats baseStats)
            {
                targetStats.ChangeFrom(baseStats);
                foreach (var v in GetPercentValues())
                    targetStats.ModifyStat(v.Key, x => x + x * v.Value);
                foreach (var v in GetAddValues())
                    targetStats.ModifyStat(v.Key, x => x + v.Value);
            }

            public void Calculate(Key key, Stats targetStats, Stats baseStats)
            {
                targetStats.ModifyStat(key, x => baseStats.GetStat(key) * (percentValues.GetStat(key) + 1));
                targetStats.ModifyStat(key, x => x + addValues.GetStat(key));
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("percent\n");
                sb.Append(percentValues.ToString()).Append("\n");
                sb.Append("add\n");
                sb.Append(addValues.ToString());
                return sb.ToString();
            }

            public string Save()
            {
                var casterData = new StringListWrapper();
                foreach(var c in casterInfo)
                {
                    var infoData = new StringListWrapper();
                    foreach(var i in c.Value as IDictionary<Key, Info>)
                        infoData.datas.Add($"{i.Key}\'{i.Value.add.Value},{i.Value.percent.Value}");
                    casterData.datas.Add($"{c.Key}\'{JsonUtility.ToJson(infoData)}");
                }
                return JsonUtility.ToJson(casterData);
            }

            public void Load(string data)
            {
                if(string.IsNullOrEmpty(data)) return;
                var casterData = JsonUtility.FromJson<StringListWrapper>(data);
                foreach(var c in casterData.datas)
                {
                    var caster = c[0..c.IndexOf('\'')];
                    var infoJson = c[(c.IndexOf('\'')+1)..];
                    var infoData = JsonUtility.FromJson<StringListWrapper>(infoJson);
                    foreach(var i in infoData.datas)
                    {
                        var key = Enum.Parse<Key>(i[0..i.IndexOf('\'')]);
                        var values = i[(i.IndexOf('\'')+1)..];
                        var addValue = float.Parse(values.Split(',')[0]);
                        var percentValue = float.Parse(values.Split(',')[1]);
                        Set
                        (
                            caster,
                            key,
                            x => percentValue,
                            x => addValue
                        );
                    }
                }
            }
        }
    }
}