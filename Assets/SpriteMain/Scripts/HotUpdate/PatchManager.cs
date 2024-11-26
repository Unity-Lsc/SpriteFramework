using System.Collections;
using System.IO;
using UnityEngine;
using YooAsset;
using SpriteFramework;

namespace SpriteMain
{
    /// <summary>
    /// 热更新管理器
    /// </summary>
    public class PatchManager : Singleton<PatchManager>
    {

        /// <summary>
        /// 资源系统运行模式
        /// </summary>
        private EPlayMode mPlayMode = EPlayMode.EditorSimulateMode;

        /// <summary>
        /// 远端地址
        /// </summary>
        private string mRemoteURL = "http://127.0.0.1:6080/";

        /// <summary>
        /// 资源包版本号
        /// </summary>
        private string mPackageVersion;

        /// <summary>
        /// 资源下载器
        /// </summary>
        private ResourceDownloaderOperation mDownloader;

        public void Init(EPlayMode mode, string url) {
            mPlayMode = mode;
            mRemoteURL = url;
        }

        /// <summary>
        /// 开始资源热更新流程
        /// </summary>
        public IEnumerator GameHotUpdate() {
            LoadPatchWindow();
            yield return InitPackage();
            yield return GetPackageVersion();
            yield return UpdateManifest();
            yield return DownloadFiles();
            yield return ClearCache();
        }

        #region 热更新流程

        /// <summary>
        /// 加载更新面板
        /// </summary>
        private void LoadPatchWindow() {
            //var go = Resources.Load<GameObject>("PatchWindow");
            //GameObject.Instantiate(go);
        }

        /// <summary>
        /// 初始化资源包
        /// </summary>
        private IEnumerator InitPackage() {
            //yield return new WaitForSeconds(0.5f);

            var playMode = mPlayMode;

            //创建默认的资源包
            string packageName = SFConstDefine.DefaultPackageName;
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null) {
                package = YooAssets.CreatePackage(packageName);
            }
            YooAssets.SetDefaultPackage(package);

            //编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode) {
                var createParameters = new EditorSimulateModeParameters {
                    SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName)
                };
                initializationOperation = package.InitializeAsync(createParameters);
            }

            //单机运行模式
            if (playMode == EPlayMode.OfflinePlayMode) {
                var createParameters = new OfflinePlayModeParameters {
                    DecryptionServices = new GameDecryptionServices(),
                    BuildinRootDirectory = Application.streamingAssetsPath
                };
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 联机运行模式
            if (playMode == EPlayMode.HostPlayMode) {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                var createParameters = new HostPlayModeParameters {
                    DecryptionServices = new GameDecryptionServices(),
                    QueryServices = new GameQueryServices(),
                    RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer)
                };
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // WebGL运行模式
            if (playMode == EPlayMode.WebPlayMode) {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                var createParameters = new WebPlayModeParameters {
                    DecryptionServices = new GameDecryptionServices(),
                    QueryServices = new GameQueryServices(),
                    RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer)
                };
                initializationOperation = package.InitializeAsync(createParameters);
            }

            yield return initializationOperation;
            if (initializationOperation.Status == EOperationStatus.Succeed) {
                Debug.Log("InitPackage Success!");
            } else {
                Debug.LogWarning($"{initializationOperation.Error}");
            }

        }

        /// <summary>
        /// 获取资源包的版本号
        /// </summary>
        private IEnumerator GetPackageVersion() {
            yield return new WaitForSecondsRealtime(0.5f);

            var package = YooAssets.GetPackage(SFConstDefine.DefaultPackageName);
            var operation = package.UpdatePackageVersionAsync();
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed) {
                mPackageVersion = operation.PackageVersion;
                Debug.Log($"远端最新版本为: {operation.PackageVersion}");
            } else {
                Debug.LogWarning(operation.Error);
            }

        }

        /// <summary>
        /// 更新资源清单
        /// </summary>
        private IEnumerator UpdateManifest() {
            yield return new WaitForSecondsRealtime(0.5f);

            bool savePackageVersion = true;
            var package = YooAssets.GetPackage(SFConstDefine.DefaultPackageName);
            var operation = package.UpdatePackageManifestAsync(mPackageVersion, savePackageVersion);
            yield return operation;

            if (operation.Status == EOperationStatus.Succeed) {
                Debug.Log("UpdateManifest Success !");
            } else {
                Debug.LogWarning(operation.Error);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private IEnumerator DownloadFiles() {
            yield return new WaitForSecondsRealtime(0.5f);

            int downloadingMaxNum = 10;//同时下载的最大文件数
            int failedTryAgain = 3;//下载失败的重试次数
            var downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            mDownloader = downloader;

            if (downloader.TotalDownloadCount == 0) {
                Debug.Log("Not found any download files !");
                yield break;
            } else {
                Debug.Log($"Found total {downloader.TotalDownloadCount} files that need download ！");

                // 注册下载回调
                downloader.OnDownloadErrorCallback = OnDownloadError;
                downloader.OnDownloadProgressCallback = OnDownloadProgress;
                downloader.OnDownloadOverCallback = OnDownloadOver;
                downloader.OnStartDownloadFileCallback = OnStartDownloadFile;
                downloader.BeginDownload();
                yield return downloader;

                // 检测下载结果
                if (downloader.Status != EOperationStatus.Succeed)
                    yield break;
            }

            yield break;
        }

        /// <summary>
        /// 清理未使用的缓存文件
        /// </summary>
        private IEnumerator ClearCache() {
            var package = YooAssets.GetPackage(SFConstDefine.DefaultPackageName);
            var operation = package.ClearUnusedCacheFilesAsync();
            yield return operation;
        }

        #endregion 热更新流程end


        #region 下载器回调相关

        /// <summary>
        /// 下载进度发生变化回调
        /// </summary>
        /// <param name="totalDownloadCount">下载资源的总数量</param>
        /// <param name="currentDownloadCount">当前下载的资源数量</param>
        /// <param name="totalDownloadBytes">下载资源的总大小</param>
        /// <param name="currentDownloadBytes">当前下载资源的大小</param>
        private void OnDownloadProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes) {

        }

        /// <summary>
        /// 文件下载失败回调
        /// </summary>
        /// <param name="fileName">下载失败的文件名称</param>
        /// <param name="error">下载失败的错误信息</param>
        private void OnDownloadError(string fileName, string error) {

        }

        /// <summary>
        /// 下载器结束回调
        /// </summary>
        /// <param name="isSucceed">是否下载成功</param>
        private void OnDownloadOver(bool isSucceed) {

        }

        /// <summary>
        /// 下载某个文件的回调
        /// </summary>
        /// <param name="fileName">下载的文件名称</param>
        /// <param name="sizeBytes">下载的文件大小</param>
        private void OnStartDownloadFile(string fileName, long sizeBytes) {

        }

        #endregion 下载器回调相关end


        #region 初始化资源包相关

        /// <summary>
        /// 获取远程服务器地址
        /// </summary>
        /// <returns></returns>
        private string GetHostServerURL() {
            //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
            string hostServerIP = "http://127.0.0.1";
            string appVersion = "v1.0";

#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/CDN/Android/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/CDN/IPhone/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return $"{hostServerIP}/CDN/WebGL/{appVersion}";
            else
                return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
		if (Application.platform == RuntimePlatform.Android)
			return $"{hostServerIP}/CDN/Android/{appVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{hostServerIP}/CDN/IPhone/{appVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{hostServerIP}/CDN/WebGL/{appVersion}";
		else
			return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
        }

        /// <summary>
        /// 资源文件解密服务类
        /// </summary>
        private class GameDecryptionServices : IDecryptionServices
        {
            public uint GetManagedReadBufferSize() {
                return 1024;
            }

            public ulong LoadFromFileOffset(DecryptFileInfo fileInfo) {
                return 32;
            }

            public byte[] LoadFromMemory(DecryptFileInfo fileInfo) {
                return null;
            }

            public Stream LoadFromStream(DecryptFileInfo fileInfo) {
                BundleStream bundleStream = new BundleStream(fileInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return bundleStream;
            }
        }

        /// <summary>
        /// 游戏查询服务
        /// </summary>
        private class GameQueryServices : IQueryServices
        {
            public const string RootFolderName = "yoo";

            public DeliveryFileInfo GetDeliveryFileInfo(string packageName, string fileName) {
                throw new System.NotImplementedException();
            }

            public bool QueryDeliveryFiles(string packageName, string fileName) {
                return false;
            }

            private static bool FileExists(string packageName, string fileName) {
                string filePath = Path.Combine(Application.streamingAssetsPath, RootFolderName, packageName, fileName);
                return File.Exists(filePath);
            }

            public bool QueryStreamingAssets(string packageName, string fileName) {
                // 注意：fileName包含文件格式
                return FileExists(packageName, fileName);
            }
        }

        /// <summary>
        /// 远程服务
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string mDefaultHostServer;
            private readonly string mFallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer) {
                mDefaultHostServer = defaultHostServer;
                mFallbackHostServer = fallbackHostServer;
            }
            string IRemoteServices.GetRemoteMainURL(string fileName) {
                return $"{mDefaultHostServer}/{fileName}";
            }
            string IRemoteServices.GetRemoteFallbackURL(string fileName) {
                return $"{mFallbackHostServer}/{fileName}";
            }
        }

        #endregion 初始化资源包相关end

    }
}
