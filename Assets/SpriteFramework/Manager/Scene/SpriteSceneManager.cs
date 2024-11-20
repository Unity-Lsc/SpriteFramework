using System.Collections;
using YooAsset;

namespace SpriteFramework
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public class SpriteSceneManager
    {

        /// <summary>
        /// 加载并进入场景
        /// </summary>
        /// <param name="sceneName">进入的场景名称</param>
        public IEnumerator LoadScene(string sceneName) {
            var handler = GameEntry.Resource.LoadSceneAsync(SFConstDefine.SceneRoot + sceneName);
            
            //加载中 显示进度
            while (!handler.IsDone) {
                float progress = handler.Progress;
                GameEntry.Log("Loading progress:{0}%", progress * 100);
                yield return null;
            }

            //加载完毕
            if(handler.Status == EOperationStatus.Succeed) {
                GameEntry.Log("Scene:{0} loaded successfully!", sceneName);
            } else {
                GameEntry.LogError("Scene:{0} load failed!", sceneName);
            }

        }

    }
}
