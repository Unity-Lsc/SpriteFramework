using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 游戏物体对象池 实现类
    /// </summary>
    public class PrefabPool
    {
        /// <summary>
        /// 预制体
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// 已被取池的对象集合
        /// </summary>
        private LinkedList<GameObject> m_SpawnedList;

        /// <summary>
        /// 在池内的对象
        /// </summary>
        private LinkedList<GameObject> m_DespawnedList;

        /// <summary>
        /// 是否开启缓存池自动清理
        /// </summary>
        private bool m_IsAutoClean;

        /// <summary>
        /// 对象池常驻数量(自动清理时,始终保留不清理的数量)
        /// </summary>
        private int m_ResidentCount;

        /// <summary>
        /// 自动清理的时间间隔
        /// </summary>
        private int m_CleanInterval;

        /// <summary>
        /// 每次自动清理的数量
        /// </summary>
        private int m_PerCleanCount;

        /// <summary>
        /// 定时清理协程是否正在运行中
        /// </summary>
        private bool m_IsCleanActive = false;

        private Transform m_Root;

        /// <summary>
        /// 当前对象的最大数量（包括在池内的, 已被取池的）
        /// </summary>
        public int TotalCount {
            get {
                int count = 0;
                count += m_SpawnedList.Count;
                count += m_DespawnedList.Count;
                return count;
            }
        }

        /// <summary>
        /// 实例化对象池
        /// </summary>
        /// <param name="prefab">池中的预制体</param>
        /// <param name="isAutoClean">对象池是否自动清理</param>
        /// <param name="residentCount">对象池中的常驻数量</param>
        /// <param name="cleanInterval">对象池自动清理的时间间隔</param>
        /// <param name="perCleanCount">对象池每次清理的数量</param>
        public PrefabPool(GameObject prefab, bool isAutoClean = true, int residentCount = 0, int cleanInterval = 10, int perCleanCount = 5) {
            this.Prefab = prefab;
            this.m_IsAutoClean = isAutoClean;
            this.m_ResidentCount = residentCount;
            this.m_CleanInterval = cleanInterval;
            this.m_PerCleanCount = perCleanCount;

            m_DespawnedList = new LinkedList<GameObject>();
            m_SpawnedList = new LinkedList<GameObject>();
        }

        public void PreloadPool(Transform root) {
            m_Root = root;
            // 初始化填充对象池
            if (m_ResidentCount > 0) {
                for (int i = 0; i < m_ResidentCount; i++) {
                    //使用InstanceHandler，预加载克隆对象
                    GameObject obj = GameObject.Instantiate(Prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(m_Root);
                    m_DespawnedList.AddLast(obj);
#if UNITY_EDITOR
                    //对象名字后缀
                    obj.name += TotalCount.ToString("#000");
#endif
                }
            }
        }

        /// <summary>
        /// 创建新的对象
        /// </summary>
        private GameObject CreateNewObject() {
            //使用InstanceHandler，克隆对象
            GameObject obj = GameObject.Instantiate(Prefab);
            obj.transform.SetParent(m_Root);
            m_SpawnedList.AddLast(obj);
#if UNITY_EDITOR
            //对象名字后缀
            obj.name += TotalCount.ToString("#000");
#endif
            return obj;
        }


        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public GameObject DequeueObj() {
            GameObject obj;
            if(m_DespawnedList.Count == 0) {
                //池中无可用对象，创建新对象
                obj = CreateNewObject();
            } else {
                //从池里拿对象
                obj = m_DespawnedList.First.Value;
                m_DespawnedList.RemoveFirst();

                if (obj == null) {
                    GameEntry.Log("池内拿出来的对象是null， 被私自Destroy了, Prefab:{0}", LogCategory.Normal, Prefab.name);
                    return null;
                }

                m_SpawnedList.AddLast(obj);
                obj.SetActive(true);
            }
            return obj;
        }

        /// <summary>
        /// 对象回池
        /// </summary>
        public bool EnqueueObj(GameObject obj) {
            m_SpawnedList.Remove(obj);
            m_DespawnedList.AddLast(obj);

            obj.SetActive(false);

            if (!m_IsCleanActive && m_IsAutoClean && TotalCount > m_ResidentCount) {
                m_IsCleanActive = true;
                GameEntry.Instance.StartCoroutine(CullDespawned());
            }
            return true;
        }

        /// <summary>
        /// 定时清理对象池的协程
        /// </summary>
        internal IEnumerator CullDespawned() {
            while (TotalCount > m_ResidentCount) {
                yield return new WaitForSeconds(m_CleanInterval);
                //每次清理几个
                for (int i = 0; i < m_PerCleanCount; i++) {
                    //保留几个对象
                    if (TotalCount <= m_ResidentCount) break;
                    if (m_DespawnedList.Count == 0) break;

                    GameObject obj = m_DespawnedList.Last.Value;
                    m_DespawnedList.RemoveLast();
                    GameObject.Destroy(obj);
                }
            }
            m_IsCleanActive = false;
            yield return null;
        }

        /// <summary>
        /// 直接销毁某个物体
        /// </summary>
        public void Destroy(GameObject obj) {
            bool isRemove = m_SpawnedList.Remove(obj);
            if (isRemove) {
                GameEntry.LogError("对象:{0}不在池中", obj.name);
            }
            GameObject.Destroy(obj);
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        internal void ClearPool() {
            List<GameObject> destroyList = new List<GameObject>();
            foreach (GameObject inst in m_DespawnedList) {
                destroyList.Add(inst);
            }
            foreach (GameObject inst in m_SpawnedList) {
                destroyList.Add(inst);
            }
            m_DespawnedList.Clear();
            m_SpawnedList.Clear();

            //先用destroyList装起来, 然后再销毁, 防止DestroyInstance内触发委托拿到的despawnedList.Count有问题
            foreach (GameObject inst in destroyList) {
                if (inst != null) {
                    GameObject.Destroy(inst);
                }
            }
            Prefab = null;
        }

    }
}
