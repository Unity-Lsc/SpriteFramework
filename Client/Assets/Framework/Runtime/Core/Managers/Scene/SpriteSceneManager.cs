using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpriteFramework
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public class SpriteSceneManager
    {
        /// <summary>
        /// 场景加载器链表
        /// </summary>
        private readonly LinkedList<SceneLoaderRoutine> m_SceneLoaderList;

        /// <summary>
        /// 目标的进度
        /// </summary>
        private readonly Dictionary<string, float> m_TargetProgressDic;

        /// <summary>
        /// 当前加载任务的业务标识
        /// </summary>
        private string m_CurrSceneTag;

        /// <summary>
        /// 当前待加载的场景资源路径列表
        /// </summary>
        private List<string> m_CurrentSceneAssetPaths;

        /// <summary>
        /// 场景是否加载中
        /// </summary>
        private bool m_CurrSceneIsLoading;

        /// <summary>
        /// 当前进度
        /// </summary>
        private float m_CurrProgress = 0;

        /// <summary>
        /// 加载完毕委托
        /// </summary>
        private Action m_OnComplete = null;

        internal SpriteSceneManager()
        {
            m_SceneLoaderList = new LinkedList<SceneLoaderRoutine>();
            m_TargetProgressDic = new Dictionary<string, float>();

            //监听单个场景加载完毕
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode sceneMode) =>
            {
                if (m_SceneLoaderList.Count == 0) return;
                foreach (var item in m_SceneLoaderList)
                {
                    if (item.SceneFullPath == scene.path)
                    {
                        //设置列表里的第一个场景为主场景(激活场景)
                        if (scene.path == m_SceneLoaderList.First.Value.SceneFullPath)
                        {
                            SceneManager.SetActiveScene(scene);
                            //初始化对象池
                            GameEntry.Pool.GameObjectPool.InitScenePool();
                        }

                        m_TargetProgressDic[scene.path] = 1;
                        break;
                    }
                }

            };

        }

        /// <summary>
        /// 异步加载场景资源列表
        /// 业务层可以把“组名、关卡名、章节名”等语义先解析成路径列表，再交给框架执行
        /// </summary>
        public UniTask LoadSceneAsync(IList<string> sceneAssetPaths, string sceneTag = null)
        {
            UniTaskCompletionSource task = new UniTaskCompletionSource();
            LoadSceneAction(sceneAssetPaths, sceneTag, () => task.TrySetResult());
            return task.Task;
        }

        /// <summary>
        /// 同步入口包装的场景加载方法
        /// </summary>
        public void LoadSceneAction(IList<string> sceneAssetPaths, string sceneTag = null, Action onComplete = null)
        {
            if (m_CurrSceneIsLoading)
            {
                GameEntry.LogError(ELogCategory.Scene, $"Scene is loading. currentTag={m_CurrSceneTag}");
                return;
            }

            if (sceneAssetPaths == null || sceneAssetPaths.Count == 0)
            {
                GameEntry.LogError(ELogCategory.Scene, "sceneAssetPaths is empty.");
                return;
            }

            m_CurrSceneIsLoading = true;
            m_OnComplete = onComplete;
            m_CurrProgress = 0;
            m_TargetProgressDic.Clear();
            m_CurrSceneTag = sceneTag ?? string.Join(",", sceneAssetPaths);
            m_CurrentSceneAssetPaths = new List<string>(sceneAssetPaths);

            if (m_SceneLoaderList.Count > 0)
            {
                foreach (SceneLoaderRoutine routine in m_SceneLoaderList)
                {
                    routine.UnLoadScene();
                }
                m_SceneLoaderList.Clear();
            }

            LoadNewScene();
        }

        /// <summary>
        /// 根据当前缓存的场景路径列表发起真正的加载
        /// </summary>
        private void LoadNewScene()
        {
            var operation = GameEntry.Loader.DefaultPackage.UnloadUnusedAssetsAsync();
            operation.WaitForAsyncComplete();

            for (int i = 0; i < m_CurrentSceneAssetPaths.Count; i++)
            {
                string sceneAssetPath = m_CurrentSceneAssetPaths[i];
                m_TargetProgressDic[sceneAssetPath] = 0;

                SceneLoaderRoutine routine = new SceneLoaderRoutine();
                m_SceneLoaderList.AddLast(routine);
                routine.LoadScene(sceneAssetPath, (string sceneFullPath, float progress) => {
                    m_TargetProgressDic[sceneFullPath] = progress;
                });
            }
        }

        public event Action<float> LoadingUpdateAction;

        internal void OnUpdate()
        {
            if (!m_CurrSceneIsLoading)
            {
                return;
            }

            LinkedListNode<SceneLoaderRoutine> curr = m_SceneLoaderList.First;
            while (curr != null)
            {
                curr.Value.OnUpdate();
                curr = curr.Next;
            }

            //模拟加载进度条
            float targetProgress = GetCurrTotalProgress();
            if (m_CurrProgress < targetProgress)
            {
                //根据实际情况调节速度, 加载已完成和未完成, 模拟进度增值速度分开计算
                m_CurrProgress += Time.deltaTime * (targetProgress < 1 ? 0.5f : 0.8f);
                m_CurrProgress = Mathf.Min(m_CurrProgress, targetProgress);
                LoadingUpdateAction?.Invoke(m_CurrProgress);
            }

            if (m_CurrProgress >= 1)
            {
                GameEntry.Log(ELogCategory.Scene, $"Scene load complete. tag={m_CurrSceneTag}");
                m_CurrSceneIsLoading = false;
                m_OnComplete?.Invoke();
            }
        }

        /// <summary>
        /// 获取当前加载的总进度
        /// </summary>
        private float GetCurrTotalProgress()
        {
            if (m_TargetProgressDic.Count == 0) return 0;

            float progress = 0;
            Dictionary<string, float>.Enumerator enumerator = m_TargetProgressDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                progress += enumerator.Current.Value;
            }

            progress /= m_TargetProgressDic.Count;
            return progress;
        }

    }
}