using TMPro;
using UnityEngine;

public class SelectLvDisplayer : MonoBehaviour
{
    //serivce
    private SelectManager selectManager => ServiceProvider.Get<SelectManager>();

    private ISelectable target;
    private TextMeshProUGUI text;
    
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        selectManager.OnSelect += select => target = select;
    }

    private void Update()
    {
        if(target != null)
            text.text = $"Lv. {target.Lv}";
    }
}