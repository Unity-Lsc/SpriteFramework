using System;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 定时器
    /// </summary>
    public class TimeAction
    {

        /// <summary>
        /// 临时目标时间点
        /// </summary>
        private float m_TillTime;

        /// <summary>
        /// 当前循环次数
        /// </summary>
        private int m_CurrLoop;

        /// <summary>
        /// 间隔（秒）
        /// </summary>
        private float m_Interval;

        /// <summary>
        /// 目标循环次数(-1表示 无限循环 0也会循环一次)
        /// </summary>
        private int m_Loop;

        /// <summary>
        /// 最后暂停时间
        /// </summary>
        private float m_LastPauseTime;

        /// <summary>
        /// 是否无视时间缩放 TODO
        /// </summary>
        private bool m_IsUnscaled;

        /// <summary>
        /// 运行中的回调 回调参数表示剩余次数
        /// </summary>
        public Action<int> OnUpdateCallback { get; private set; }

        /// <summary>
        /// 运行完毕的回调
        /// </summary>
        public Action OnCompleteCallback { get; private set; }

        /// <summary>
        /// 用于判断委托方是不是为null，如果为null则不调用委托， 避免报错
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="target">定时器的目标</param>
        /// <param name="interval">定时器时间间隔(秒)</param>
        /// <param name="loop">循环次数</param>
        /// <param name="onUpdate">运行中的回调</param>
        /// <param name="onComplete">运行完毕的回调</param>
        /// <param name="isUnScaled">是否忽略时间缩放(默认不忽略)</param>
        internal TimeAction Init(object target, float interval, int loop, Action<int> onUpdate, Action onComplete, bool isUnScaled) {
            if(m_TillTime > 0) {
                GameEntry.Log("定时器正在使用中");
                return null;
            }

            Target = target;
            m_IsUnscaled = isUnScaled;
            m_Interval = interval;
            m_TillTime = (m_IsUnscaled ? Time.unscaledTime : Time.time) + m_Interval;
            m_Loop = loop;
            OnUpdateCallback = onUpdate;
            OnCompleteCallback = onComplete;

            m_CurrLoop = 0;
            GameEntry.Time.Register(m_TillTime, this, isUnScaled);
            return this;
        }

        /// <summary>
        /// 定时器停止
        /// </summary>
        public void Stop() {
            //防止重复停止
            if (m_TillTime == 0) return;
            GameEntry.Time.Remove(m_TillTime, this, m_IsUnscaled);
            OnUpdateCallback = null;
            OnCompleteCallback = null;
            m_TillTime = 0;
        }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        public void Pause() {
            m_LastPauseTime = m_IsUnscaled ? Time.unscaledTime : Time.time;
            GameEntry.Time.Remove(m_TillTime, this, m_IsUnscaled);
        }

        /// <summary>
        /// 恢复计时器
        /// </summary>
        public void Resume() {
            //暂停的时间
            float deltaTime = (m_IsUnscaled ? Time.unscaledTime : Time.time) - m_LastPauseTime;
            m_TillTime += deltaTime;
            GameEntry.Time.Register(m_TillTime, this, m_IsUnscaled);
        }

        /// <summary>
        /// 时间到达
        /// </summary>
        public void TillTimeEnd() {
            //以下代码 间隔m_Interval 时间 执行一次
            if (Target == null) {
                GameEntry.LogWarning("TimeAction.OnUpdateCallback.Target==null");
                return;
            }
            m_CurrLoop++;
            OnUpdateCallback?.Invoke(m_Loop - m_CurrLoop);

            //-1表示无限次循环, 那么永远不会执行OnCompleteCallback
            if (m_CurrLoop >= m_Loop && m_Loop != -1) {
                if (Target == null) {
                    GameEntry.LogWarning("TimeAction.OnUpdateCallback.Target==null");
                    return;
                }
                //完成了，执行OnCompleteAction，结束循环
                OnCompleteCallback?.Invoke();
            } 
            //继续循环
            else {
                m_TillTime = (m_IsUnscaled ? Time.unscaledTime : Time.time) + m_Interval;
                GameEntry.Time.Register(m_TillTime, this, m_IsUnscaled);
            }
        }

    }
}
