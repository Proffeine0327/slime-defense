using UnityEngine;

namespace Game.Services
{
    public class LobbyManager : MonoBehaviour
    {
        //Services
        private DataContext dataContext => ServiceProvider.Get<DataContext>();

        public void SelectStage(int index)
        {
            
        }

        private void Awake()
        {
            ServiceProvider.Register(this);
        }
    }
}