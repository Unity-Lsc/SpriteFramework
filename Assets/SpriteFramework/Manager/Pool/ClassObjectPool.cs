using SpriteMain;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 类对象池
    /// </summary>
    public class ClassObjectPool
    {
        /// <summary>
        /// 存储常驻数量的集合(参数int:对象类型的哈希值 byte:常驻数量)
        /// </summary>
        public Dictionary<int, byte> ResidentCountDict { get; private set; }

        /// <summary>
        /// 类对象池字典(参数int:对象类型的哈希值)
        /// </summary>
        private Dictionary<int, Queue<object>> _classObjectPoolDict;

        /// <summary>
        /// 下次释放类对象运行时间
        /// </summary>
        public float ReleaseNextRunTime { get; private set; }

        public ClassObjectPool() {
            ResidentCountDict = new Dictionary<int, byte>();
            _classObjectPoolDict = new Dictionary<int, Queue<object>>();

            ReleaseNextRunTime = Time.time;

            //初始化常用类的常驻数量
            SetResidentCount<HttpRoutine>(3);
            SetResidentCount<Dictionary<string, object>>(3);
        }

        internal void OnUpdate() {
            if(_classObjectPoolDict.Count > 0 && Time.time > ReleaseNextRunTime + MainEntry.ParamsSettings.PoolReleaseClassObjectInterval) {
                ReleaseNextRunTime = Time.time;
                ReleasePool();
            }
        }
        
        /// <summary>
        /// 设置常驻数量
        /// </summary>
        /// <typeparam name="T">要设置常驻数量的对象类型</typeparam>
        /// <param name="count">要设置的常驻数量</param>
        public void SetResidentCount<T>(byte count) where T : class {
            int key = typeof(T).GetHashCode();
            ResidentCountDict[key] = count;
        }

        /// <summary>
        /// 取出一个对象
        /// </summary>
        public T Dequeue<T>() where T : class, new() {
            lock (_classObjectPoolDict) {
                //先获取类的哈希值
                int key = typeof(T).GetHashCode();
                _classObjectPoolDict.TryGetValue(key, out Queue<object> queue);
                if (queue == null) {
                    queue = new Queue<object>();
                    _classObjectPoolDict[key] = queue;
                }

                //开始获取对象 >0说明队列中有闲置的 <=0则表示队列中没有,需要进行实例化
                if(queue.Count > 0) {
                    object obj = queue.Dequeue();
                    return (T)obj;
                } else {
                    return new T();
                }

            }
        }

        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="obj">要回池的对象</param>
        public void Enqueue(object obj) {
            lock (_classObjectPoolDict) {
                int key = obj.GetType().GetHashCode();
                _classObjectPoolDict.TryGetValue(key, out Queue<object> queue);
                if(queue != null) {
                    queue.Enqueue(obj);
                }
            }
        }

        /// <summary>
        /// 释放类对象池
        /// </summary>
        public void ReleasePool() {
            lock (_classObjectPoolDict) {
                int queueCount = 0;
                var enumerator = _classObjectPoolDict.GetEnumerator();
                while (enumerator.MoveNext()) {
                    int key = enumerator.Current.Key;
                    Queue<object> queue = _classObjectPoolDict[key];
                    queueCount = queue.Count;
                    byte residentCount = 0;
                    ResidentCountDict.TryGetValue(key, out residentCount);
                    while (queueCount > residentCount) {
                        //队列中有可释放的对象
                        queueCount--;
                        queue.Dequeue();//从队列中取出一个 这个对象没有任何引用，就变成了野指针 等待GC回收
                    }
                }
            }
        }

        public void Dispose() {
            ResidentCountDict.Clear();
            _classObjectPoolDict.Clear();
        }

    }
}
