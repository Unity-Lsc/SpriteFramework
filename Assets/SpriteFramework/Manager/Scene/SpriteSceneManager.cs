using System;
using System.Collections;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public class SpriteSceneManager
    {
        /// <summary>
        /// 加载进度的回调
        /// </summary>
        public event Action<float> OnLoadingUpdateCallback;

        /// <summary>
        /// 场景是否加载中
        /// </summary>
        private bool m_CurrSceneIsLoading;

        /// <summary>
        /// 目标加载进度
        /// </summary>
        private float m_TargetProgress = 0f;

        /// <summary>
        /// 当前显示的进度
        /// </summary>
        private float m_CurProgress = 0f;

        /// <summary>
        /// 进度条更新速度
        /// </summary>
        private float m_ProgressSpeed = 0.5f; // 可以根据需要调整

        /// <summary>
        /// 当前场景的名字
        /// </summary>
        private string m_SceneName;

        /// <summary>
        /// 场景加载结束的回调
        /// </summary>
        private Action m_OnComplete;

        /// <summary>
        /// 加载并进入场景
        /// </summary>
        /// <param name="sceneName">进入的场景名称</param>
        public IEnumerator LoadScene(string sceneName, Action onComplete = null) {
            if (m_CurrSceneIsLoading) {
                GameEntry.LogError("场景:{0}正在加载中", sceneName);
                yield break;
            }

            m_OnComplete = onComplete;
            if(m_SceneName == sceneName) {
                GameEntry.LogError("重复加载场景:{0}", sceneName);
                m_OnComplete?.Invoke();
                yield break;
            }

            m_CurrSceneIsLoading = true;
            m_SceneName = sceneName;
            

            //显示Loading界面
            GameEntry.UI.OpenUIForm<UILoadingForm>();

            var handler = GameEntry.Resource.LoadSceneAsync(SFConstDefine.SceneRoot + sceneName);
            
            //加载中 显示进度
            while (!handler.IsDone) {
                m_TargetProgress = handler.Progress;
                yield return null;
            }

            m_TargetProgress = 1.0f;

        }

        internal void OnUpdate() {
            if (!m_CurrSceneIsLoading) return;
            if(m_CurProgress < m_TargetProgress) {
                m_CurProgress += Time.deltaTime * m_ProgressSpeed;

                //防止进度超过100%，显示出现例如102%这种情况
                m_CurProgress = Mathf.Min(m_CurProgress, m_TargetProgress);
                OnLoadingUpdateCallback?.Invoke(m_CurProgress);
            }
            if(m_TargetProgress == 1 && Mathf.Abs(m_CurProgress - m_TargetProgress) < 0.001f) {
                GameEntry.Log("场景:{0} 加载完毕!", m_SceneName);
                m_CurrSceneIsLoading = false;
                m_OnComplete?.Invoke();
            }
        }

    }
}
