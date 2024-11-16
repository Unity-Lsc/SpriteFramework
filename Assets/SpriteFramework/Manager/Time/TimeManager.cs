using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SpriteFramework
{
    /// <summary>
    /// 定时器管理器
    /// </summary>
    public class TimeManager
    {
        /// <summary>
        /// 定时器集合(可排序)
        /// </summary>
        private readonly SortedDictionary<float, List<TimeAction>> m_SortedDict;
        /// <summary>
        /// 定时器集合(忽略时间缩放,可排序)
        /// </summary>
        private readonly SortedDictionary<float, List<TimeAction>> m_UnscaledSortedDict;

        private readonly Queue<float> m_TimeOutQueue;

        // 记录最近的TimeAction的临时目标时间点
        private float m_MinTime;
        private float m_UnscaledMinTime;

        public TimeManager() {
            m_SortedDict = new SortedDictionary<float, List<TimeAction>>();
            m_UnscaledSortedDict = new SortedDictionary<float, List<TimeAction>>();
            m_TimeOutQueue = new Queue<float>();
        }

        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="tillTime">临时目标时间点</param>
        /// <param name="timeAction">注册的定时器</param>
        /// <param name="isUnScaled">是否忽略时间缩放(默认不忽略)</param>
        internal void Register(float tillTime, TimeAction timeAction, bool isUnScaled) {
            if (isUnScaled) {
                if (!m_UnscaledSortedDict.ContainsKey(tillTime))
                    m_UnscaledSortedDict.Add(tillTime, new List<TimeAction>());
                m_UnscaledSortedDict[tillTime].Add(timeAction);
                if (tillTime < m_UnscaledMinTime)
                    m_UnscaledMinTime = tillTime;
            } else {
                if(m_SortedDict.TryGetValue(tillTime, out List<TimeAction> lst)) {
                    lst.Add(timeAction);
                } else {
                    lst = new List<TimeAction>();
                    lst.Add(timeAction);
                    m_SortedDict.Add(tillTime, lst);
                }
                if (tillTime < m_MinTime)
                    m_MinTime = tillTime;
            }
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="tillTime">临时目标时间点</param>
        /// <param name="timeAction">移除的定时器</param>
        /// <param name="isUnscaled">是否忽略时间缩放(默认不忽略)</param>
        internal void Remove(float tillTime, TimeAction timeAction, bool isUnscaled) {
            if (isUnscaled) {
                if(m_UnscaledSortedDict.TryGetValue(tillTime, out List<TimeAction> lst)) {
                    lst.Remove(timeAction);
                }
            } else {
                if(m_SortedDict.TryGetValue(tillTime, out List<TimeAction> lst)) {
                    lst.Remove(timeAction);
                }
            }
        }

        /// <summary>
        /// 定时器集合运行
        /// </summary>
        internal void OnUpdate() {
            if(m_SortedDict.Count > 0 && Time.time >= m_MinTime) {
                var enumerator = m_SortedDict.GetEnumerator();
                while (enumerator.MoveNext()) {
                    float k = enumerator.Current.Key;
                    if(k > Time.time) {
                        m_MinTime = k;
                        break;
                    }
                    m_TimeOutQueue.Enqueue(k);
                }
                foreach (var item in m_TimeOutQueue) {
                    if(m_SortedDict.TryGetValue(item, out List<TimeAction> lst)) {
                        for (int i = 0; i < lst.Count; i++) {
                            lst[i].TillTimeEnd();
                        }
                        m_SortedDict.Remove(item);
                    }
                }
                m_TimeOutQueue.Clear();
            }

            if(m_UnscaledSortedDict.Count > 0 && Time.time >= m_UnscaledMinTime) {
                var enumerator = m_UnscaledSortedDict.GetEnumerator();
                while (enumerator.MoveNext()) {
                    float k = enumerator.Current.Key;
                    if(k > Time.unscaledTime) {
                        m_UnscaledMinTime = k;
                        break;
                    }
                    m_TimeOutQueue.Enqueue(k);
                }
                foreach (var item in m_TimeOutQueue) {
                    if (!m_UnscaledSortedDict.ContainsKey(item)) continue;
                    var lst = m_UnscaledSortedDict[item];
                    for (int i = 0; i < lst.Count; i++) {
                        lst[i].TillTimeEnd();
                    }
                    m_UnscaledSortedDict.Remove(item);
                }
                m_TimeOutQueue.Clear();
            }
        }

        /// <summary>
        /// 创建定时器，可循环多次
        /// </summary>
        /// <param name="interval">间隔时间</param>
        /// <param name="loop">循环次数(默认1次)</param>
        /// <param name="onUpdate">循环一次时调用</param>
        /// <param name="onComplete">全部循环完毕时调用</param>
        /// <param name="unScaled">是否无视Time.timeScale</param>
        /// <returns>定时器</returns>
        public TimeAction CreateTimer(object target, float interval, int loop= 1, Action<int> onUpdate = null, Action onComplete = null, bool unScaled = false) {
            return new TimeAction().Init(target, interval, loop, onUpdate, onComplete, unScaled);
        }

        /// <summary>
        /// 创建定时器，定时n秒后执行一次
        /// </summary>
        /// <param name="delayTime">延时时间</param>
        /// <param name="onComplete">延时结束回调</param>
        /// <param name="unScaled">是否无视Time.timeScale</param>
        /// <returns>定时器</returns>
        public TimeAction CreateTimerOnce(object target, float delayTime, Action onComplete = null, bool unScaled = false) {
            return new TimeAction().Init(target, delayTime, 1, null, onComplete, unScaled);
        }

        public UniTask Delay(object target, float delayTime, bool unScaled = false) {
            var task = new UniTaskCompletionSource();
            CreateTimerOnce(target, delayTime, () => task.TrySetResult(), unScaled);
            return task.Task;
        }

        /// <summary>
        /// 延迟一帧
        /// </summary>
        public void Yield(Action onComplete) {
            GameEntry.Instance.StartCoroutine(YieldCoroutine());
            IEnumerator YieldCoroutine() {
                yield return null;
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// 设置时间缩放
        /// </summary>
        /// <param name="scale">时间缩放值</param>
        public void SetTimeScale(float scale) {
            Time.timeScale = scale;
        }

    }
}
