using System;
using System.Collections;
using UnityEngine;
using SpriteFramework;


/// <summary>
/// GameObject池, 对象定时自动回池
/// </summary>
public class GameObjectDespawnHandle : MonoBehaviour, IGameObjectPoolLifecycle
{
    private Coroutine coroutine;
    private float DelayTimeDespawn;
    private bool isNotifyingDespawn;

    public event Action OnDespawn;

    public void SetDelayTimeDespawn(float delayTime)
    {
        DelayTimeDespawn = delayTime;
        if (DelayTimeDespawn < 0)
        {
            GameEntry.LogError(ELogCategory.Pool, "DelayTimeDespawn不允许小于0");
            return;
        }
        if (DelayTimeDespawn == 0)
        {
            Despawn();
            return;
        }

        StopTime();
        coroutine = StartCoroutine(DelayDespawn());
    }

    private void StopTime()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void Despawn()
    {
        StopTime();
        GameEntry.Pool.GameObjectPool.Despawn(gameObject);
    }

    void IGameObjectPoolLifecycle.OnSpawnedFromPool()
    {
        StopTime();
    }

    void IGameObjectPoolLifecycle.OnDespawnedFromPool()
    {
        if (isNotifyingDespawn) return;

        isNotifyingDespawn = true;
        StopTime();

        Action onDespawn = OnDespawn;
        OnDespawn = null;

        try
        {
            onDespawn?.Invoke();
        }
        finally
        {
            isNotifyingDespawn = false;
        }
    }

    IEnumerator DelayDespawn()
    {
        yield return new WaitForSeconds(DelayTimeDespawn);
        Despawn();
    }

}
