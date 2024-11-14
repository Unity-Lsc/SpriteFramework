using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventMgr : UnitySingleton<EventMgr>, IBaseManager
    {

        public delegate void OnActionHandler(object param);

        /// <summary>
        /// 事件回调的存储列表
        /// </summary>
        public Dictionary<ushort, LinkedList<OnActionHandler>> mEventDict;

        public void Init() {
            Boot.Instance.RegisterBaseManager(this);
            mEventDict = new Dictionary<ushort, LinkedList<OnActionHandler>>();
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        public void AddListener(ushort key, OnActionHandler handler) {
            mEventDict.TryGetValue(key, out LinkedList<OnActionHandler> handlerLst);
            if (handlerLst == null) {
                handlerLst = new LinkedList<OnActionHandler>();
                mEventDict[key] = handlerLst;
            }
            handlerLst.AddLast(handler);
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        public void RemoveListener(ushort key, OnActionHandler handler) {
            mEventDict.TryGetValue(key, out LinkedList<OnActionHandler> handlerLst);
            if (handlerLst != null) {
                handlerLst.Remove(handler);
                if (handlerLst.Count <= 0) {
                    mEventDict.Remove(key);
                }
            }
        }

        /// <summary>
        /// 移除某个key下的所有事件监听
        /// </summary>
        public void RemoveListener(ushort key) {
            mEventDict.TryGetValue(key, out LinkedList<OnActionHandler> handlerLst);
            if (handlerLst != null) {
                for (var curNode = handlerLst.First; curNode != null; curNode = curNode.Next) {
                    handlerLst.Remove(curNode.Value);
                }
                mEventDict.Remove(key);
            }
        }

        /// <summary>
        /// 事件派发
        /// </summary>
        public void Dispatch(ushort key, object param = null) {
            mEventDict.TryGetValue(key, out LinkedList<OnActionHandler> handlerLst);
            if (handlerLst != null) {
                for (var curNode = handlerLst.First; curNode != null; curNode = curNode.Next) {
                    var handler = curNode.Value;
                    handler?.Invoke(param);
                }
            }
        }

        public void Dispose() {
            mEventDict.Clear();
        }

    }
}
