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
                //caster add
                casterInfo
                    .ObserveAdd()
                    .Subscribe(kvp =>
                    {
                        //on dictionary added key
                        kvp.Value
                            .ObserveAdd()
                            .Subscribe(e =>
                            {
                                var key = e.Key;
                                var percent = e.Value.percent;
                                var add = e.Value.add;

                                //apply value when changed
                                percent
                                    .Pairwise()
                                    .Subscribe(p =>
                                    {
                                        var oldValue = p.Previous;
                                        var newValue = p.Current;
                                        percentValues.SetStat(key, x => x + (newValue - oldValue));
                                    });
                                add
                                    .Pairwise()
                                    .Subscribe(p =>
                                    {
                                        var oldValue = p.Previous;
                                        var newValue = p.Current;
                                        addValues.SetStat(key, x => x + (newValue - oldValue));
                                    });
                                percentValues.SetStat(key, x => x + percent.Value);
                                addValues.SetStat(key, x => x + add.Value);
                            });

                        //on dictionary removed key
                        kvp.Value
                            .ObserveRemove()
                            .Subscribe(e =>
                            {
                                var key = e.Key;
                                var percent = e.Value.percent.Value;
                                var add = e.Value.add.Value;

                                //apply value
                                percentValues.SetStat(key, x => x - percent);
                                addValues.SetStat(key, x => x - add);

                                //if caster affects nothing, remove caster key
                                if (casterInfo[kvp.Key].Count == 0)
                                    casterInfo.Remove(kvp.Key);
                            });

                        //on dictionary value changed
                        kvp.Value
                            .ObserveReplace()
                            .Subscribe(e =>
                            {
                                var key = e.Key;
                                var oldPercent = e.OldValue.percent.Value;
                                var oldAdd = e.OldValue.add.Value;
                                var newPercent = e.NewValue.percent.Value;
                                var newAdd = e.NewValue.add.Value;

                                percentValues.SetStat(key, x => x + (newPercent - oldPercent));
                                addValues.SetStat(key, x => x + (newAdd - oldAdd));
                            });
                    });

                casterInfo
                    .ObserveRemove()
                    .Subscribe(kvp =>
                    {
                        foreach (var info in kvp.Value)
                        {
                            percentValues.SetStat(info.Key, x => x - info.Value.percent.Value);
                            addValues.SetStat(info.Key, x => x - info.Value.add.Value);
                        }
                    });

                percentValues.OnStatChanged += (key, _, _) => OnValueChange?.Invoke(key);
                addValues.OnStatChanged += (key, _, _) => OnValueChange?.Invoke(key);
            }

            /// <summary>
            /// add/change modify infomation
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
                //if caster has never used this before
                if (!casterInfo.ContainsKey(caster))
                {
                    //if percent and add value is 0, return
                    if (add(0) == 0 && percent(0) == 0) return;
                    
                    casterInfo.Add(caster, new());
                    casterInfo[caster].Add(key, new(add(0), percent(0)));
                    return;
                }

                //if caster used this before, but not this key
                if (!casterInfo[caster].ContainsKey(key))
                {
                    //if percent and add value is 0, return
                    if (add(0) == 0 && percent(0) == 0) return;

                    casterInfo[caster].Add(key, new(add(0), percent(0)));
                    return;
                }


                var info = casterInfo[caster][key];
                //if final modified value is 0, remove key
                if (add(info.add.Value) == 0 && percent(info.percent.Value) == 0)
                {
                    casterInfo[caster].Remove(key);
                    return;
                }

                //apply modified value
                info.add.Value = add(info.add.Value);
                info.percent.Value = percent(info.percent.Value);
            }

            /// <summary>
            /// apply modified value to target stats <br/>
            /// flow) base -calculate-> target 
            /// </summary>
            /// <param name="target">target apply stats</param>
            /// <param name="base">base stats</param>
            public void CalculateAll(Stats target, Stats @base)
            {
                target.ChangeFrom(@base);

                for (int i = 0; i < (int)Key.End; i++)
                    Calculate((Key)i, target, @base);
            }

            /// <summary>
            /// apply modified value to target stat <br/>
            /// flow) base -calculate-> target
            /// </summary>
            /// <param name="key">target calculate key</param>
            /// <param name="target">target apply stats</param>
            /// <param name="base">base stats</param>
            public void Calculate(Key key, Stats target, Stats @base)
            {
                target.SetStat(key, x => @base.GetStat(key) * (percentValues.GetStat(key) + 1));
                target.SetStat(key, x => x + addValues.GetStat(key));
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
                foreach (var c in casterInfo)
                {
                    var infoData = new StringListWrapper();
                    foreach (var i in c.Value as IDictionary<Key, Info>)
                        infoData.datas.Add($"{i.Key}\'{i.Value.add.Value},{i.Value.percent.Value}");
                    casterData.datas.Add($"{c.Key}\'{JsonUtility.ToJson(infoData)}");
                }
                return JsonUtility.ToJson(casterData);
            }

            public void Load(string data)
            {
                if (string.IsNullOrEmpty(data)) return;
                var casterData = JsonUtility.FromJson<StringListWrapper>(data);
                foreach (var c in casterData.datas)
                {
                    var caster = c[0..c.IndexOf('\'')];
                    var infoJson = c[(c.IndexOf('\'') + 1)..];
                    var infoData = JsonUtility.FromJson<StringListWrapper>(infoJson);
                    foreach (var i in infoData.datas)
                    {
                        var key = Enum.Parse<Key>(i[0..i.IndexOf('\'')]);
                        var values = i[(i.IndexOf('\'') + 1)..];
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