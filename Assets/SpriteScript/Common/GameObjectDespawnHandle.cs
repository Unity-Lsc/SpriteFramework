using System;
using UnityEngine;
using SpriteFramework;
using System.Collections;

/// <summary>
/// GameObject池, 对象定时自动回池
/// </summary>
public class GameObjectDespawnHandle : MonoBehaviour
{

    private Coroutine m_Coroutine;

    /// <summary>
    /// 回池的延迟时间
    /// </summary>
    private float m_EnqueueDelayTime;

    /// <summary>
    /// 回池后的回调
    /// </summary>
    public Action OnComplete;

    /// <summary>
    /// 设置对象自动回池的延迟时间
    /// </summary>
    /// <param name="delayTime">延迟时间</param>
    public void SetEnqueueDelayTime(float delayTime) {
        if(delayTime < 0) {
            GameEntry.LogError("DelayTime不允许小于0");
            return;
        }
        if(delayTime == 0) {
            GameEntry.Pool.GameObjectPool.Enqueue(gameObject);
            OnComplete?.Invoke();
            return;
        }
        m_EnqueueDelayTime = delayTime;
        if(m_Coroutine != null) {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
        m_Coroutine = StartCoroutine(DelayEnqueue());
    }

    IEnumerator DelayEnqueue() {
        yield return new WaitForSeconds(m_EnqueueDelayTime);
        GameEntry.Pool.GameObjectPool.Enqueue(gameObject);
        OnComplete?.Invoke();
    }

}
