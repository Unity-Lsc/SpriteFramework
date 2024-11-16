using System.IO;
using YooAsset;

namespace SpriteFramework
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResManager
    {

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        public AssetOperationHandle LoadAssetAsync<T>(string assetPath, string packageName = null) where T : UnityEngine.Object {
            if(packageName == null) {
                var handle = YooAssets.LoadAssetAsync<T>(assetPath);
                return handle;
            }

            var package = YooAssets.TryGetPackage(packageName);
            if(package == null) {
                return null;
            }

            return package.LoadAssetAsync<T>(assetPath); 
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        public T LoadAsset<T>(string assetPath, string packageName = null) where T : UnityEngine.Object {
            T assetObj = null;
            AssetOperationHandle handle;

            if (packageName == null) {
                handle = YooAssets.LoadAssetSync<T>(assetPath);
            } else {
                var package = YooAssets.TryGetPackage(packageName);
                if (package == null) {
                    return null;
                }
                handle = package.LoadAssetSync<T>(assetPath);
            }

            if (handle != null) {
                assetObj = handle.AssetObject as T;
                handle.Dispose();
            }
            return assetObj;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="scenePath">场景路径</param>
        public SceneOperationHandle LoadSceneAsync(string scenePath, string packageName = null) {
            packageName ??= SFConstDefine.DefaultPackageName;
            var package = YooAssets.TryGetPackage(packageName);
            if(package == null) {
                return null;
            }
            return package.LoadSceneAsync(scenePath);
        }

        #region 封装的具体加载资源的方法

        /// <summary>
        /// 异步加载配置文件
        /// </summary>
        /// <param name="dataTableName">加载的配置文件名字</param>
        public AssetOperationHandle LoadDataTableAsync<T>(string dataTableName, string packageName = null) where T : UnityEngine.Object {
            string dataTablePath = Path.Combine(SFConstDefine.DataTableRoot, dataTableName);
            return LoadAssetAsync<T>(dataTablePath,packageName);
        }

        /// <summary>
        /// 异步加载声音文件
        /// </summary>
        /// <param name="audioName">加载的声音文件名字</param>
        public AssetOperationHandle LoadSoundAsync<T>(string audioName, string packageName = null) where T : UnityEngine.Object {
            string soundPath = Path.Combine(SFConstDefine.SoundRoot, audioName);
            return LoadAssetAsync<T>(soundPath, packageName);
        }

        /// <summary>
        /// 同步加载配置文件
        /// </summary>
        /// <param name="dataTableName">加载的配置文件名字</param>
        public T LoadDataTable<T>(string dataTableName, string packageName = null) where T : UnityEngine.Object {
            string dataTablePath = Path.Combine(SFConstDefine.DataTableRoot, dataTableName);
            return LoadAsset<T>(dataTablePath, packageName);
        }

        /// <summary>
        /// 同步加载声音文件
        /// </summary>
        /// <param name="soundName">加载的声音文件名字</param>
        public T LoadSound<T>(string soundName, string packageName = null) where T : UnityEngine.Object {
            string soundPath = Path.Combine(SFConstDefine.SoundRoot, soundName);
            return LoadAsset<T>(soundPath, packageName);
        }

        #endregion 封装的具体加载资源的方法end

        /// <summary>
        /// 卸载未使用的资源
        /// </summary>
        public void UnloadUnusedAssets(string packageName = null) {
            packageName ??= SFConstDefine.DefaultPackageName;
            var package = YooAssets.TryGetPackage(packageName);
            if(package == null) {
                return;
            }
            package.UnloadUnusedAssets();
        }
    }
}
