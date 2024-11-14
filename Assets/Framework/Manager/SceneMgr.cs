using System.Collections;
using UnityEngine;
using YooAsset;

namespace SpriteFramework
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public class SceneMgr : UnitySingleton<SceneMgr>, IBaseManager
    {

        public void Init() {
            Boot.Instance.RegisterBaseManager(this);
        }

        /// <summary>
        /// 加载并进入场景
        /// </summary>
        /// <param name="sceneName">进入的场景名称</param>
        public IEnumerator LoadScene(string sceneName) {
            var handler = ResMgr.Instance.LoadSceneAsync(SFConstDefine.SceneRoot + sceneName);
            
            //加载中 显示进度
            while (!handler.IsDone) {
                float progress = handler.Progress;
                Debug.Log($"Loading progress: {progress * 100}%");
                yield return null;
            }

            //加载完毕
            if(handler.Status == EOperationStatus.Succeed) {
                Debug.LogFormat("Scene:{0} loaded successfully!", sceneName);
            } else {
                Debug.LogErrorFormat("Scene:{0} load failed!", sceneName);
            }

        }

        public void Dispose() {

        }

    }
}
