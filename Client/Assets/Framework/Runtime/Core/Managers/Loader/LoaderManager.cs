using UnityEngine;
using Object = UnityEngine.Object;
using Cysharp.Threading.Tasks;
using YooAsset;
using System;


namespace SpriteFramework
{
    /// <summary>
    /// 资源加载 管理器
    /// </summary>
    public class LoaderManager
    {
        public ResourcePackage DefaultPackage { get; private set; }

        public LoaderManager()
        {
            DefaultPackage = YooAssets.GetPackage("DefaultPackage");

            //加载时间最短, 但加载时帧率下降最严重
            Application.backgroundLoadingPriority = ThreadPriority.High;
        }

        /// <summary>
        /// 异步加载主资源，自动加载依赖
        /// 注意: 如果这个资源没有打AB包, 则无法加载
        /// </summary>
        /// <typeparam name="T">加载的资源类型</typeparam>
        /// <param name="assetFullPath">资源路径</param>
        /// <param name="target">依赖的游戏物体,它销毁时会触发引用计数减少</param>
        public async UniTask<T> LoadMainAssetAsync<T>(string assetFullPath, GameObject target) where T : Object
        {
            if (target == null)
            {
                GameEntry.LogError(ELogCategory.Loader, "依赖的游戏物体不可为空");
                return null;
            }

            var op = DefaultPackage.LoadAssetAsync(assetFullPath);
            await op;
            AssetReleaseHandle.Add(op, target);
            return op.AssetObject as T;
        }

        /// <summary>
        /// 异步加载主资源，自动加载依赖
        /// 注意: 如果这个资源没有打AB包, 则无法加载
        /// </summary>
        /// <typeparam name="T">加载的资源类型</typeparam>
        /// <param name="assetFullPath">资源路径</param>
        /// <param name="target">依赖的游戏物体,它销毁时会触发引用计数减少</param>
        /// <param name="callback">加载完成的回调</param>
        /// <param name="onFailed">加载失败的回调</param>
        public void LoadMainAssetAsync<T>(string assetFullPath, GameObject target, Action<AssetHandle, T> callback, Action<string> onFailed = null) where T : Object
        {
            if (string.IsNullOrEmpty(assetFullPath))
            {
                onFailed?.Invoke($"AssetFullPath:{assetFullPath} is null or empty");
                callback?.Invoke(null, null);
                return;
            }

            bool isGameObjectType = typeof(T) == typeof(GameObject);
            if (!isGameObjectType && target == null)
            {
                string err = "依赖的游戏物体不可为空";
                GameEntry.LogError(ELogCategory.Loader, err);
                onFailed?.Invoke(err);
                callback?.Invoke(null, null);
                return;
            }
            var op = DefaultPackage.LoadAssetAsync(assetFullPath);
            op.Completed += handle =>
            {
                if (handle.Status != EOperationStatus.Succeed)
                {
                    string err = $"资源加载失败: {assetFullPath}";
                    GameEntry.LogError(ELogCategory.Loader, err);
                    onFailed?.Invoke(err);
                    callback?.Invoke(handle, null);
                    return;
                }

                T asset = handle.AssetObject as T;

                //Prefab做特殊处理，不绑定target(绑定实例化后的资源实体)
                if(isGameObjectType)
                {
                    callback?.Invoke(handle, asset);
                    return;
                }

                AssetReleaseHandle.Add(handle, target);
                callback?.Invoke(handle,asset);
            };
        }

        /// <summary>
        /// 同步加载主资源, 自动加载依赖
        /// 注意: 如果这个资源没有打AB包, 则无法加载
        /// 注意: 微信小游戏不支持同步加载
        /// </summary>
        /// <param name="target">依赖的游戏物体， 它销毁时会触发引用计数减少</param>
        public T LoadMainAsset<T>(string assetFullPath, GameObject target) where T : Object
        {
            if (target == null)
            {
                GameEntry.LogError(ELogCategory.Loader, "依赖的游戏物体不可为空");
                return null;
            }
            
            var op = DefaultPackage.LoadAssetSync(assetFullPath);
            AssetReleaseHandle.Add(op, target);
            return op.AssetObject as T;
        }

    }
}