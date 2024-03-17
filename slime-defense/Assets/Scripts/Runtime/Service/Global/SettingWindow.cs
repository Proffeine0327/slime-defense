using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Services
{
    public class SettingWindow : MonoBehaviour
    {
        //services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        [SerializeField] private Image bg;
        [SerializeField] private Slider masterSoundSlider;
        [SerializeField] private TextMeshProUGUI masterSoundPercentText;
        [SerializeField] private Slider bgSoundSlider;
        [SerializeField] private TextMeshProUGUI bgSoundPercentText;
        [SerializeField] private Slider effectSoundSlider;
        [SerializeField] private TextMeshProUGUI effectSoundPercentText;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown hzDropdown;
        [SerializeField] private Button exitButton;

        private Action onClose;
        private Vector2Int[] resolutions;
        private RefreshRate[] refreshRates;

        private SettingData settingData => dataContext.userData.settingData;

        private void Awake()
        {
            ServiceProvider.Register(this, true);

            Debug.Log(settingData.masterSound);

            masterSoundSlider.onValueChanged.AddListener(x => settingData.masterSound = x);
            bgSoundSlider.onValueChanged.AddListener(x => settingData.bgSound = x);
            effectSoundSlider.onValueChanged.AddListener(x => settingData.effectSound = x);

            var maxRes = Screen.resolutions
                .Select(r => new Vector2Int(r.width, r.height))
                .OrderByDescending(x => x.x)
                .ThenByDescending(x => x.y)
                .First();
            resolutions = new Vector2Int[]
            {
                maxRes,
                new(1920,1080),
                new(1600,900),
                new(1366,768),
                new(1152,648),
                new(1024,576),
                new(1280,720),
                new(960,540),
                new(864,486),
                new(800,450),
                new(640,360)
            };
            resolutions = resolutions
                .Where(r => r.x <= maxRes.x && r.y <= maxRes.y && Mathf.Abs((float)r.x / r.y - 16f / 9) < 0.01f)
                .Distinct()
                .ToArray();
            foreach (var r in resolutions)
            {
                var option = new TMP_Dropdown.OptionData { text = $"{r.x}x{r.y}" };
                resolutionDropdown.options.Add(option);
            }
            resolutionDropdown.onValueChanged.AddListener(x => settingData.resolution = x);

            var maxRate = Screen.resolutions
                .Select(r => r.refreshRateRatio)
                .OrderByDescending(r => r.value)
                .First();
            refreshRates = new RefreshRate[]
            {
                maxRate,
                new() { numerator = 60, denominator = 1 },
                new() { numerator = 30, denominator = 1 },
                new() { numerator = 15, denominator = 1 },
            };
            refreshRates = refreshRates
                .Where(r => r.value <= maxRate.value)
                .ToArray();
            foreach (var hz in refreshRates)
            {
                var option = new TMP_Dropdown.OptionData { text = hz.value.ToString() };
                hzDropdown.options.Add(option);
            }
            hzDropdown.onValueChanged.AddListener(x => settingData.hz = x);
            exitButton.onClick.AddListener(() =>
            {
                Hide();
                onClose?.Invoke();
            });

            settingData
                .ObserveEveryValueChanged(x => x.masterSound)
                .Subscribe(x =>
                {
                    masterSoundSlider.value = x;
                    masterSoundPercentText.text = $"{x * 100:##0}%";
                });
            settingData
                .ObserveEveryValueChanged(x => x.bgSound)
                .Subscribe(x =>
                {
                    bgSoundSlider.value = x;
                    bgSoundPercentText.text = $"{x * 100:##0}%";
                });
            settingData
                .ObserveEveryValueChanged(x => x.effectSound)
                .Subscribe(x =>
                {
                    effectSoundSlider.value = x;
                    effectSoundPercentText.text = $"{x * 100:##0}%";
                });
            settingData
                .ObserveEveryValueChanged(x => x.resolution)
                .Subscribe(x =>
                {
                    resolutionDropdown.value = x;
                    Screen.SetResolution
                    (
                        resolutions[settingData.resolution].x,
                        resolutions[settingData.resolution].y,
                        FullScreenMode.ExclusiveFullScreen,
                        refreshRates[settingData.hz]
                    );
                });
            settingData
                .ObserveEveryValueChanged(x => x.hz)
                .Subscribe(x =>
                {
                    hzDropdown.value = x;
                    Screen.SetResolution
                    (
                        resolutions[settingData.resolution].x,
                        resolutions[settingData.resolution].y,
                        FullScreenMode.ExclusiveFullScreen,
                        refreshRates[settingData.hz]
                    );
                });
        }

        public void Display(Action onClose = null)
        {
            this.onClose = onClose;
            bg.gameObject.SetActive(true);
        }

        private void Hide()
        {
            bg.gameObject.SetActive(false);
            dataContext.userData.Save();
            onClose?.Invoke();
        }
    }
}