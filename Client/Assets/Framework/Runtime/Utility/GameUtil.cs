using UnityEngine;
using SpriteFramework;
using Cysharp.Threading.Tasks;
using System;

public class GameUtil
{

    /// <summary>
    /// 实例化已加载好的Prefab
    /// </summary>  
    /// <param name="prefab">预制体资源</param>
    /// <param name="parent">父节点</param>
    public static GameObject ClonePrefab(GameObject prefab, Transform parent = null)
    {
        if (prefab == null)
        {
            GameEntry.LogError(ELogCategory.Framework, "InstantiatePrefab failed: prefab is null");
            return null;
        }

        GameObject obj = UnityEngine.Object.Instantiate(prefab, parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        return obj;
    }

    /// <summary>
    /// 加载Prefab并实例化(UniTask版)
    /// </summary>
    public static UniTask<GameObject> LoadPrefabCloneAsync(string prefabFullPath, Transform parent = null)
    {
        var taskCompletionSource = new UniTaskCompletionSource<GameObject>();
        LoadPrefabClone(prefabFullPath, parent, obj => taskCompletionSource.TrySetResult(obj), error => taskCompletionSource.TrySetException(new Exception(error)));

        return taskCompletionSource.Task;
    }

    /// <summary>
    /// 加载Prefab并实例化(回调版)
    /// </summary>
    /// <param name="prefabFullPath">资源路径</param>
    /// <param name="parent">父节点</param>
    /// <param name="onSuccess">加载成功回调</param>
    /// <param name="onFailed">加载失败回调</param>
    public static void LoadPrefabClone(string prefabFullPath, Transform parent, Action<GameObject> onSuccess, Action<string> onFailed = null)
    {
        GameEntry.Loader.LoadMainAssetAsync<GameObject>(prefabFullPath, null,
            (handle, prefab) => {
                if (prefab == null)
                {
                    onFailed?.Invoke($"Prefab load failed: {prefabFullPath}");
                    onSuccess?.Invoke(null);
                    return;
                }

                GameObject obj = ClonePrefab(prefab, parent);
                if(obj == null)
                {
                    onFailed?.Invoke($"Prefab instantiate failed: {prefabFullPath}");
                    onSuccess?.Invoke(null);
                    return;
                }
                AssetReleaseHandle.Add(handle, obj);//绑定生命周期引用（防止被回收）
                onSuccess?.Invoke(obj);
            },
            error => {
                onFailed?.Invoke(error);
                onSuccess?.Invoke(null);
            });
    }

}