using System;
using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager
    {
        /// <summary>
        /// 事件回调的存储字典
        /// </summary>
        private readonly Dictionary<ushort, object> m_EventDict;

        public EventManager()
        {
            m_EventDict = new Dictionary<ushort, object>();
        }

        #region 添加事件监听

        /// <summary>
        /// 添加事件监听（无参数）
        /// </summary>
        public void AddListener(ushort key, Action handler)
        {
            GetOrCreateHandlerList<Action>(key).AddLast(handler);
        }

        /// <summary>
        /// 添加事件监听（1个参数）
        /// </summary>
        public void AddListener<T1>(ushort key, Action<T1> handler)
        {
            GetOrCreateHandlerList<Action<T1>>(key).AddLast(handler);
        }

        /// <summary>
        /// 添加事件监听（2个参数）
        /// </summary>
        public void AddListener<T1, T2>(ushort key, Action<T1, T2> handler)
        {
            GetOrCreateHandlerList<Action<T1, T2>>(key).AddLast(handler);
        }

        /// <summary>
        /// 添加事件监听（3个参数）
        /// </summary>
        public void AddListener<T1, T2, T3>(ushort key, Action<T1, T2, T3> handler)
        {
            GetOrCreateHandlerList<Action<T1, T2, T3>>(key).AddLast(handler);
        }

        /// <summary>
        /// 添加事件监听（4个参数）
        /// </summary>
        public void AddListener<T1, T2, T3, T4>(ushort key, Action<T1, T2, T3, T4> handler)
        {
            GetOrCreateHandlerList<Action<T1, T2, T3, T4>>(key).AddLast(handler);
        }

        #endregion

        #region 移除事件监听

        /// <summary>
        /// 移除事件监听（无参数）
        /// </summary>
        public void RemoveListener(ushort key, Action handler)
        {
            RemoveHandler<Action>(key, handler);
        }

        /// <summary>
        /// 移除事件监听（1个参数）
        /// </summary>
        public void RemoveListener<T1>(ushort key, Action<T1> handler)
        {
            RemoveHandler<Action<T1>>(key, handler);
        }

        /// <summary>
        /// 移除事件监听（2个参数）
        /// </summary>
        public void RemoveListener<T1, T2>(ushort key, Action<T1, T2> handler)
        {
            RemoveHandler<Action<T1, T2>>(key, handler);
        }

        /// <summary>
        /// 移除事件监听（3个参数）
        /// </summary>
        public void RemoveListener<T1, T2, T3>(ushort key, Action<T1, T2, T3> handler)
        {
            RemoveHandler<Action<T1, T2, T3>>(key, handler);
        }

        /// <summary>
        /// 移除事件监听（4个参数）
        /// </summary>
        public void RemoveListener<T1, T2, T3, T4>(ushort key, Action<T1, T2, T3, T4> handler)
        {
            RemoveHandler<Action<T1, T2, T3, T4>>(key, handler);
        }

        /// <summary>
        /// 移除某个key下的所有事件监听
        /// </summary>
        public void RemoveListener(ushort key)
        {
            m_EventDict.Remove(key);
        }

        #endregion

        #region 事件派发

        /// <summary>
        /// 事件派发（无参数）
        /// </summary>
        public void Dispatch(ushort key)
        {
            if (m_EventDict.TryGetValue(key, out object handlerListObj))
            {
                if (handlerListObj is LinkedList<Action> handlerList)
                {
                    for (var curNode = handlerList.First; curNode != null; curNode = curNode.Next)
                    {
                        curNode.Value?.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// 事件派发（1个参数）
        /// </summary>
        public void Dispatch<T1>(ushort key, T1 param1)
        {
            if (m_EventDict.TryGetValue(key, out object handlerListObj))
            {
                if (handlerListObj is LinkedList<Action<T1>> handlerList)
                {
                    for (var curNode = handlerList.First; curNode != null; curNode = curNode.Next)
                    {
                        curNode.Value?.Invoke(param1);
                    }
                }
            }
        }

        /// <summary>
        /// 事件派发（2个参数）
        /// </summary>
        public void Dispatch<T1, T2>(ushort key, T1 param1, T2 param2)
        {
            if (m_EventDict.TryGetValue(key, out object handlerListObj))
            {
                if (handlerListObj is LinkedList<Action<T1, T2>> handlerList)
                {
                    for (var curNode = handlerList.First; curNode != null; curNode = curNode.Next)
                    {
                        curNode.Value?.Invoke(param1, param2);
                    }
                }
            }
        }

        /// <summary>
        /// 事件派发（3个参数）
        /// </summary>
        public void Dispatch<T1, T2, T3>(ushort key, T1 param1, T2 param2, T3 param3)
        {
            if (m_EventDict.TryGetValue(key, out object handlerListObj))
            {
                if (handlerListObj is LinkedList<Action<T1, T2, T3>> handlerList)
                {
                    for (var curNode = handlerList.First; curNode != null; curNode = curNode.Next)
                    {
                        curNode.Value?.Invoke(param1, param2, param3);
                    }
                }
            }
        }

        /// <summary>
        /// 事件派发（4个参数）
        /// </summary>
        public void Dispatch<T1, T2, T3, T4>(ushort key, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            if (m_EventDict.TryGetValue(key, out object handlerListObj))
            {
                if (handlerListObj is LinkedList<Action<T1, T2, T3, T4>> handlerList)
                {
                    for (var curNode = handlerList.First; curNode != null; curNode = curNode.Next)
                    {
                        curNode.Value?.Invoke(param1, param2, param3, param4);
                    }
                }
            }
        }

        #endregion

        #region 私有辅助方法

        /// <summary>
        /// 获取或创建指定类型的处理器列表
        /// </summary>
        private LinkedList<T> GetOrCreateHandlerList<T>(ushort key)
        {
            if (!m_EventDict.TryGetValue(key, out object handlerListObj))
            {
                var handlerList = new LinkedList<T>();
                m_EventDict[key] = handlerList;
                return handlerList;
            }

            if (handlerListObj is LinkedList<T> existingList)
            {
                return existingList;
            }

            // 如果已存在但类型不匹配，抛出异常
            throw new InvalidOperationException($"事件键 {key} 已存在，但委托类型不匹配。期望类型: {typeof(T).Name}");
        }

        /// <summary>
        /// 移除指定类型的处理器
        /// </summary>
        private void RemoveHandler<T>(ushort key, T handler)
        {
            if (m_EventDict.TryGetValue(key, out object handlerListObj))
            {
                if (handlerListObj is LinkedList<T> handlerList)
                {
                    handlerList.Remove(handler);
                    if (handlerList.Count <= 0)
                    {
                        m_EventDict.Remove(key);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 清理所有事件监听
        /// </summary>
        public void Dispose()
        {
            m_EventDict.Clear();
        }

    }
}