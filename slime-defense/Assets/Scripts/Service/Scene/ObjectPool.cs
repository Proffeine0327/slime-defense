using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Services
{
    public class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class PrefabObjectPoolInfo
        {
            public GameObject prefab;
            public int maxAmount;
        }

        [System.Serializable]
        public class GroupObjectPoolInfo
        {
            public string path;
            public int maxAmount;
        }

        public class ObjectPoolQueueInfo
        {
            public ObjectPoolQueueInfo(string key, Transform parent, GameObject prefab, int maxAmount)
            {
                this.parent = parent;
                this.prefab = prefab;
                this.maxAmount = maxAmount;

                queue = new();
                for (int i = 0; i < maxAmount; i++)
                {
                    var obj = CreateNewInstance(prefab, key);
                    obj.transform.SetParent(parent);
                    obj.SetActive(false);

                    queue.Enqueue(obj);
                }
            }

            public Transform parent;
            public GameObject prefab;
            public int maxAmount;
            public Queue<GameObject> queue = new();
        }

        public PrefabObjectPoolInfo[] prefabPoolInfo;
        public GroupObjectPoolInfo[] groupPoolInfo;
        private readonly Dictionary<string, ObjectPoolQueueInfo> pools = new();

        public GameObject GetObject(string key, Vector3 position = default, Quaternion rotation = default)
        {
            if (pools[key].queue.Count == 0)
                return CreateNewInstance(pools[key].prefab, key);

            var target = pools[key].queue.Dequeue();
            target.transform.position = position;
            target.transform.rotation = rotation;
            target.transform.SetParent(null);
            target.gameObject.SetActive(true);
            return target;
        }

        public void PoolObject(string key, GameObject obj)
        {
            if (pools[key].queue.Count >= pools[key].maxAmount)
            {
                Destroy(obj.gameObject);
            }
            else
            {
                pools[key].queue.Enqueue(obj);
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(pools[key].parent);
            }
        }

        private static GameObject CreateNewInstance(GameObject prefab, string key)
        {
            var obj = Instantiate(prefab);
            Poolable poolable = obj.GetComponent<Poolable>() ?? obj.AddComponent<Poolable>();
            poolable.Initialize(key);
            return obj;
        }

        private void Awake()
        {
            ServiceProvider.Register(this);

            foreach (var info in prefabPoolInfo)
            {
                var pObj = new GameObject($"{info.prefab.name}");
                pObj.transform.SetParent(transform);
                pools.Add(info.prefab.name, new ObjectPoolQueueInfo(info.prefab.name, pObj.transform, info.prefab, info.maxAmount));
            }

            foreach (var info in groupPoolInfo)
            {
                foreach (var prefab in Resources.LoadAll<GameObject>(info.path))
                {
                    var pObj = new GameObject($"{prefab.name}");
                    pObj.transform.SetParent(transform);
                    pools.Add(prefab.name, new ObjectPoolQueueInfo(prefab.name, pObj.transform, prefab, info.maxAmount));
                }
            }
        }
    }
}