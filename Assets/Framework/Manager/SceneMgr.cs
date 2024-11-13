using UnityEngine.SceneManagement;

namespace SpriteFramework
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public class SceneMgr : UnitySingleton<SceneMgr>
    {

        public void Init() {

        }

        /// <summary>
        /// 进入场景
        /// </summary>
        /// <param name="sceneName">进入的场景名称</param>
        public void EnterScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

    }
}
