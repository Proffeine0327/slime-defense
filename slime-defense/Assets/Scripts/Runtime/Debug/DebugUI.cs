using System.Collections;
using System.Collections.Generic;
using Game.Services;
using TMPro;
using UnityEngine;

public enum DebugTarget { None, Skill, Modifier, MaxStat, CurStat }

public class DebugUI : MonoBehaviour
{
    //services
    private SlimeManager slimeManager => ServiceProvider.Get<SlimeManager>();
    private CameraManager cameraManager => ServiceProvider.Get<CameraManager>();

    [SerializeField] private TextMeshProUGUI debugTextPrefab; 
    [SerializeField] private DebugTarget debugTarget;

    private List<TextMeshProUGUI> texts = new();

    private void Update()
    {
        //match count of dispays and slimes
        while(texts.Count != slimeManager.Slimes.Count)
        {
            if(texts.Count > slimeManager.Slimes.Count)
            {
                Destroy(texts[^1].gameObject);
                texts.RemoveAt(texts.Count - 1);
            }
            else
            {
                texts.Add(Instantiate(debugTextPrefab, transform));
            }
        }

        var count = 0;
        foreach(var slime in slimeManager.Slimes)
        {
            texts[count].gameObject.SetActive(debugTarget != DebugTarget.None);
            texts[count].rectTransform.anchoredPosition = cameraManager.mainCamera.WorldToScreenPoint(slime.transform.position);
            switch (debugTarget)
            {
                case DebugTarget.Skill: texts[count].text = slime.skill.ToString(); break;
                case DebugTarget.Modifier: texts[count].text = slime.modifier.ToString(); break;
                case DebugTarget.MaxStat: texts[count].text = slime.maxStats.ToString(); break;
                case DebugTarget.CurStat: texts[count].text = slime.curStats.ToString(); break;
            }
            count++;
        }
    }
}
