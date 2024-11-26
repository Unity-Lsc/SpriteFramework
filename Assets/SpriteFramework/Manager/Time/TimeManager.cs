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
        private readonly SortedDictionary<float, List<TimeAction>> _sortedDict;
        /// <summary>
        /// 定时器集合(忽略时间缩放,可排序)
        /// </summary>
        private readonly SortedDictionary<float, List<TimeAction>> _unscaledSortedDict;

        private readonly Queue<float> _timeOutQueue;

        // 记录最近的TimeAction的临时目标时间点
        private float _minTime;
        private float _unscaledMinTime;

        public TimeManager() {
            _sortedDict = new SortedDictionary<float, List<TimeAction>>();
            _unscaledSortedDict = new SortedDictionary<float, List<TimeAction>>();
            _timeOutQueue = new Queue<float>();
        }

        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="tillTime">临时目标时间点</param>
        /// <param name="timeAction">注册的定时器</param>
        /// <param name="isUnScaled">是否忽略时间缩放(默认不忽略)</param>
        internal void Register(float tillTime, TimeAction timeAction, bool isUnScaled) {
            if (isUnScaled) {
                if (!_unscaledSortedDict.ContainsKey(tillTime))
                    _unscaledSortedDict.Add(tillTime, new List<TimeAction>());
                _unscaledSortedDict[tillTime].Add(timeAction);
                if (tillTime < _unscaledMinTime)
                    _unscaledMinTime = tillTime;
            } else {
                if(_sortedDict.TryGetValue(tillTime, out List<TimeAction> lst)) {
                    lst.Add(timeAction);
                } else {
                    lst = new List<TimeAction>();
                    lst.Add(timeAction);
                    _sortedDict.Add(tillTime, lst);
                }
                if (tillTime < _minTime)
                    _minTime = tillTime;
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
                if(_unscaledSortedDict.TryGetValue(tillTime, out List<TimeAction> lst)) {
                    lst.Remove(timeAction);
                }
            } else {
                if(_sortedDict.TryGetValue(tillTime, out List<TimeAction> lst)) {
                    lst.Remove(timeAction);
                }
            }
        }

        /// <summary>
        /// 定时器集合运行
        /// </summary>
        internal void OnUpdate() {
            if(_sortedDict.Count > 0 && Time.time >= _minTime) {
                var enumerator = _sortedDict.GetEnumerator();
                while (enumerator.MoveNext()) {
                    float k = enumerator.Current.Key;
                    if(k > Time.time) {
                        _minTime = k;
                        break;
                    }
                    _timeOutQueue.Enqueue(k);
                }
                foreach (var item in _timeOutQueue) {
                    if(_sortedDict.TryGetValue(item, out List<TimeAction> lst)) {
                        for (int i = 0; i < lst.Count; i++) {
                            lst[i].TillTimeEnd();
                        }
                        _sortedDict.Remove(item);
                    }
                }
                _timeOutQueue.Clear();
            }

            if(_unscaledSortedDict.Count > 0 && Time.time >= _unscaledMinTime) {
                var enumerator = _unscaledSortedDict.GetEnumerator();
                while (enumerator.MoveNext()) {
                    float k = enumerator.Current.Key;
                    if(k > Time.unscaledTime) {
                        _unscaledMinTime = k;
                        break;
                    }
                    _timeOutQueue.Enqueue(k);
                }
                foreach (var item in _timeOutQueue) {
                    if (!_unscaledSortedDict.ContainsKey(item)) continue;
                    var lst = _unscaledSortedDict[item];
                    for (int i = 0; i < lst.Count; i++) {
                        lst[i].TillTimeEnd();
                    }
                    _unscaledSortedDict.Remove(item);
                }
                _timeOutQueue.Clear();
            }
        }

        /// <summary>
        /// 创建定时器(可循环多次)
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
