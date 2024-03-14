using UnityEngine;

namespace Game.DeckSettingScene
{
    public class Slot : MonoBehaviour
    {
        private int index;

        public int Index => index;

        public void SetIndex(int index)
        {
            this.index = index;
        }
    }
}