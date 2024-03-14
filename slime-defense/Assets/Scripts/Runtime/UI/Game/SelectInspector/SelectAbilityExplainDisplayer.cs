using System.Collections;
using System.Collections.Generic;
using Game.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.DeckSettingScene
{
    public class SelectAbilityExplainDisplayer : PopupTrigger
    {
        //service
        private SelectManager selectManager => ServiceProvider.Get<SelectManager>();

        private Explain explain => popup as Explain;

        private void Start()
        {
            OnChangeState += b =>
            {
                var c = selectManager.CurrentSelect;
                if(c == null) return;

                if(b) explain.Display(c.Skill.Icon, c.Skill.Name, c.Skill.Explain);
                else explain.Hide();
            };
        }
    }
}