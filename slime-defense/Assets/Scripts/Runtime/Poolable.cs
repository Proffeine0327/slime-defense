using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Services;

namespace Game
{
    public class Poolable : MonoBehaviour
    {
        //services
        private ObjectPool objectPool => ServiceProvider.Get<ObjectPool>();

        private string key;

        public void Initialize(string key)
        {
            this.key = key;
        }

        public void Pool()
        {
            objectPool.PoolObject(key, gameObject);
        }
    }
}