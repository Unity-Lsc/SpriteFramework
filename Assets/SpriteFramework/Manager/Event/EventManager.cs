using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager
    {

        public delegate void OnActionHandler(object param);

        /// <summary>
        /// 事件回调的存储列表
        /// </summary>
        public Dictionary<ushort, LinkedList<OnActionHandler>> m_EventDict;

        public EventManager() {
            m_EventDict = new Dictionary<ushort, LinkedList<OnActionHandler>>();
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        public void AddListener(ushort key, OnActionHandler handler) {
            m_EventDict.TryGetValue(key, out LinkedList<OnActionHandler> handlerLst);
            if (handlerLst == null) {
                handlerLst = new LinkedList<OnActionHandler>();
                m_EventDict[key] = handlerLst;
            }
            handlerLst.AddLast(handler);
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        public void RemoveListener(ushort key, OnActionHandler handler) {
            m_EventDict.TryGetValue(key, out LinkedList<OnActionHandler> handlerLst);
            if (handlerLst != null) {
                handlerLst.Remove(handler);
                if (handlerLst.Count <= 0) {
                    m_EventDict.Remove(key);
                }
            }
        }

        /// <summary>
        /// 移除某个key下的所有事件监听
        /// </summary>
        public void RemoveListener(ushort key) {
            m_EventDict.TryGetValue(key, out LinkedList<OnActionHandler> handlerLst);
            if (handlerLst != null) {
                for (var curNode = handlerLst.First; curNode != null; curNode = curNode.Next) {
                    handlerLst.Remove(curNode.Value);
                }
                m_EventDict.Remove(key);
            }
        }

        /// <summary>
        /// 事件派发
        /// </summary>
        public void Dispatch(ushort key, object param = null) {
            m_EventDict.TryGetValue(key, out LinkedList<OnActionHandler> handlerLst);
            if (handlerLst != null) {
                for (var curNode = handlerLst.First; curNode != null; curNode = curNode.Next) {
                    var handler = curNode.Value;
                    handler?.Invoke(param);
                }
            }
        }

        public void Dispose() {
            m_EventDict.Clear();
        }

    }
}
