using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Services
{
    public class SoundManager : MonoBehaviour
    {
        //service
        private ResourceLoader resourceLoader => ServiceProvider.Get<ResourceLoader>();
        private DataContext dataContext => ServiceProvider.Get<DataContext>();
        private ScreenFade screenFade => ServiceProvider.Get<ScreenFade>();

        private AudioSource oneShotSound;
        private Dictionary<string, AudioSource> loopingSounds = new();

        private SettingData settingData => dataContext.userData.settingData;

        private void Awake()
        {
            if(!ServiceProvider.Register(this, true))
                return;

            oneShotSound = new GameObject("OneShot").AddComponent<AudioSource>();
            oneShotSound.transform.SetParent(transform);

            screenFade.OnSceneChanged += TryChangeBGM;
        }

        private void Start()
        {
            settingData.ObserveEveryValueChanged(x => x.masterSound)
                .Subscribe(_ => ApplyAllSoundsVolume());
            settingData.ObserveEveryValueChanged(x => x.bgSound)
                .Subscribe(_ => ApplyAllSoundsVolume());
        }

        private void ApplyAllSoundsVolume()
        {
            foreach (var s in loopingSounds.Values)
                s.volume = settingData.masterSound * settingData.bgSound;
        }

        private void TryChangeBGM(string pre, string cur)
        {
            if (!resourceLoader.sounds.ContainsKey(cur)) return;
            Stop(pre);
            Play(cur, true);
        }

        public void Play(string key, bool looping = false)
        {
            if (!resourceLoader.sounds.ContainsKey(key))
            {
                Debug.LogWarning($"사운드: {key}가 없습니다. 사운드가 재생되지 않았습니다.");
                return;
            }

            if (looping)
            {
                if (loopingSounds.ContainsKey(key))
                {
                    if (!loopingSounds[key].isPlaying)
                        loopingSounds[key].Play();
                }
                else
                {
                    var newSound = new GameObject(key).AddComponent<AudioSource>();
                    newSound.transform.SetParent(transform);
                    newSound.loop = true;
                    newSound.clip = resourceLoader.sounds[key];
                    newSound.volume = settingData.masterSound * settingData.bgSound;
                    newSound.Play();
                    loopingSounds.Add(key, newSound);
                }
            }
            else
            {
                oneShotSound.PlayOneShot(resourceLoader.sounds.GetValueOrDefault(key), settingData.masterSound * settingData.effectSound);
            }
        }

        public void Stop(string key)
        {
            if (!string.IsNullOrEmpty(key) && loopingSounds.ContainsKey(key) && loopingSounds[key].isPlaying)
                loopingSounds[key].Stop();
        }
    }
}
