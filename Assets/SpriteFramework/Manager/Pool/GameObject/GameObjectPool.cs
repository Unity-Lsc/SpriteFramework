using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 游戏物体对象池
    /// </summary>
    public class GameObjectPool
    {

        private Dictionary<string, PrefabPool> m_GameObjectPoolDict;

        private Transform m_Root;

        public GameObjectPool() {
            m_GameObjectPoolDict = new Dictionary<string, PrefabPool>();
            m_Root = new GameObject("GameObjectPool").transform;
            m_Root.SetParent(GameEntry.Instance.transform);
        }

        /// <summary>
        /// 通过prefab,从对象池中获取对象
        /// </summary>
        public GameObject Dequeue(GameObject prefab) {
            if(prefab == null) {
                GameEntry.LogError("prefab为空");
                return null;
            }
            string name = prefab.name;
            m_GameObjectPoolDict.TryGetValue(name, out PrefabPool prefabPool);
            if(prefabPool == null) {
                prefabPool = new PrefabPool(prefab);
                var root = new GameObject(prefab.name + "Pool");
                root.transform.SetParent(m_Root);
                prefabPool.PreloadPool(root.transform);
                m_GameObjectPoolDict[name] = prefabPool;
            }
            prefabPool.Prefab = prefab;
            GameObject obj = prefabPool.DequeueObj();
            return obj;
        }

        /// <summary>
        /// 通过资源名字,从对象池中获取对象
        /// </summary>
        /// <param name="prefabName">预制体名字</param>
        public GameObject Dequeue(string prefabName) {
            GameObject prefab = GameEntry.Resource.LoadAsset<GameObject>(SFConstDefine.PrefabRoot + prefabName);
            return Dequeue(prefab);
        }

        /// <summary>
        /// 通过资源名字,从对象池中异步获取对象
        /// </summary>
        /// <param name="prefabName">预制体名字</param>
        public async Task<GameObject> DequeueAsync(string prefabName) {
            var handler = GameEntry.Resource.LoadAssetAsync<GameObject>(SFConstDefine.PrefabRoot + prefabName);
            await handler.Task;
            var obj = handler.AssetObject as GameObject;
            return Dequeue(obj);
        }

        public void Enqueue(GameObject obj) {
            if (obj == null) return;
            string name = GetObjName(obj);
            if (m_GameObjectPoolDict.TryGetValue(name, out PrefabPool prefabPool)) {
                prefabPool.EnqueueObj(obj);
            }
        }

        /// <summary>
        /// 直接销毁对象
        /// </summary>
        public void Destroy(GameObject obj) {
            string name = GetObjName(obj);
            if(m_GameObjectPoolDict.TryGetValue(name, out PrefabPool prefabPool)) {
                prefabPool.Destroy(obj);
            } else {
                GameEntry.LogError("该对象:{0} 不存在池内", obj.name);
            }
        }

        private string GetObjName(GameObject obj) {
            return obj.name.Substring(0, obj.name.IndexOf("(Clone)"));
        }

        public void Dispose() {
            var enumerator = m_GameObjectPoolDict.GetEnumerator();
            while (enumerator.MoveNext()) {
                enumerator.Current.Value.ClearPool();
            }
            m_GameObjectPoolDict.Clear();
        }

    }
}
