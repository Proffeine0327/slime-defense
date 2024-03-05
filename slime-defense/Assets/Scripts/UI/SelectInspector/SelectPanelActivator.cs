using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPanelActivator : MonoBehaviour
{
    //serivce
    private SelectManager selectManager => ServiceProvider.Get<SelectManager>();

    [SerializeField] private GameObject panel;

    private void Start()
    {
        selectManager.OnSelect += select =>
        {
            panel.SetActive(select != null);
        };

        panel.SetActive(false);
    }
}
