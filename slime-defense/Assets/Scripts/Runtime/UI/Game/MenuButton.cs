using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GameScene
{
    public class MenuButton : MonoBehaviour
    {
        //service
        private GameMenuWindow gameMenuWindow => ServiceProvider.Get<GameMenuWindow>();

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => gameMenuWindow.Display());
        }
    }
}