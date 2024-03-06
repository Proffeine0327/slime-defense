using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SelectIconChanger : MonoBehaviour
    {
        //services
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();

        private void Start()
        {
            var image = GetComponent<Image>();
            selectManager.OnSelect += select =>
            {
                if (select != null)
                    image.sprite = select.Icon;
            };
        }
    }
}
