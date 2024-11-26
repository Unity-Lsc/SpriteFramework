using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

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
        private bool _isCurSceneLoading;

        /// <summary>
        /// 目标加载进度
        /// </summary>
        private float _targetProgress = 0f;

        /// <summary>
        /// 当前显示的进度
        /// </summary>
        private float _curProgress = 0f;

        /// <summary>
        /// 进度条更新速度
        /// </summary>
        private float _progressSpeed = 0.5f; // 可以根据需要调整

        /// <summary>
        /// 当前场景的名字
        /// </summary>
        private string _sceneName;

        /// <summary>
        /// 场景加载结束的回调
        /// </summary>
        private Action _onComplete;


        public void LoadSceneAsync(string sceneName, Action onComplete = null, LoadSceneMode loadMode = LoadSceneMode.Single) {
            if (_isCurSceneLoading) {
                GameEntry.LogError("场景:{0}正在加载中", sceneName);
                return;
            }

            _onComplete = onComplete;
            if (_sceneName == sceneName) {
                GameEntry.LogError("重复加载场景:{0}", sceneName);
                _onComplete?.Invoke();
                return;
            }

            _curProgress = 0;
            _isCurSceneLoading = true;
            _sceneName = sceneName;

            //显示Loading界面
            GameEntry.UI.OpenUIForm<UILoadingForm>();
            var handler = YooAssets.LoadSceneAsync(SFConstDefine.SceneRoot + sceneName, loadMode);
            GameEntry.Instance.StartCoroutine(TrackProgress(handler));
        }

        private IEnumerator TrackProgress(SceneOperationHandle handler) {
            //加载中 显示进度
            while (!handler.IsDone) {
                _targetProgress = handler.Progress;
                yield return null;
            }

            _targetProgress = 1.0f;
        }

        internal void OnUpdate() {
            if (!_isCurSceneLoading) return;
            if(_curProgress < _targetProgress) {
                _curProgress += Time.deltaTime * (_targetProgress < 1 ? _progressSpeed : _progressSpeed * 2);

                //防止进度超过100%，显示出现例如102%这种情况
                _curProgress = Mathf.Min(_curProgress, _targetProgress);
                OnLoadingUpdateCallback?.Invoke(_curProgress);
            }
            if(_curProgress == 1 && Mathf.Abs(_curProgress - _targetProgress) < 0.001f) {
                GameEntry.Log("场景:{0} 加载完毕!", _sceneName);
                OnLoadingUpdateCallback?.Invoke(_curProgress);
                _isCurSceneLoading = false;
                _onComplete?.Invoke();
            }
        }

    }
}
