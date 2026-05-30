using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;

namespace SpriteFramework
{
    /// <summary>
    /// Http管理器
    /// </summary>
    public class HttpManager
    {
        /// <summary>
        /// 正式账号服务器Url
        /// </summary>
        private readonly string m_WebAccountUrl;
        /// <summary>
        /// 测试账号服务器Url
        /// </summary>
        private readonly string m_TestWebAccountUrl;
        /// <summary>
        /// 是否测试环境
        /// </summary>
        private readonly bool m_IsTest;

        /// <summary>
        /// 真实账号服务器Url
        /// </summary>
        public string RealWebAccountUrl { get { return "http://" + RealIpAndPort + "/"; } }
        public string RealIpAndPort { get { return m_IsTest ? m_TestWebAccountUrl : m_WebAccountUrl; } }

        /// <summary>
        /// 请求失败事件
        /// 框架只负责把失败抛出来，至于弹窗、重试还是上报，由业务层决定
        /// </summary>
        public event Action<UnityWebRequest> RequestFailed;


        public HttpManager()
        {
            m_WebAccountUrl = GameEntry.ParamsSettings.WebAccountUrl;
            m_TestWebAccountUrl = GameEntry.ParamsSettings.TestWebAccountUrl;
            m_IsTest = GameEntry.ParamsSettings.IsTest;
        }

        public void GetArgs(string url, bool loadingCircle = false, Action<UnityWebRequest> callBack = null)
        {
            if (loadingCircle)
            {
                CircleCtrl.Instance.CircleOpen();
            }

            HttpRoutine.Create().Get(url, (ret) =>
            {
                if (loadingCircle)
                {
                    CircleCtrl.Instance.CircleClose();
                }

                if (ret.result != UnityWebRequest.Result.Success)
                {
                    RequestFailed?.Invoke(ret);
                    return;
                }
                else
                {
                    callBack?.Invoke(ret);
                }
            });
        }

        public void Get(string url, bool loadingCircle = false, Action<string> callBack = null)
        {
            GetArgs(url, loadingCircle, (args) =>
            {
                callBack?.Invoke(args.downloadHandler.text);
            });
        }

        public UniTask<UnityWebRequest> GetArgsAsync(string url, bool loadingCircle = false)
        {
            var task = new UniTaskCompletionSource<UnityWebRequest>();
            GetArgs(url, loadingCircle, x => task.TrySetResult(x));
            return task.Task;
        }

        public UniTask<string> GetAsync(string url, bool loadingCircle = false)
        {
            var task = new UniTaskCompletionSource<string>();
            Get(url, loadingCircle, x => task.TrySetResult(x));
            return task.Task;
        }

        public void PostArgs(string url, string json = null, bool loadingCircle = false, Action<UnityWebRequest> callBack = null)
        {
            if (loadingCircle)
            {
                CircleCtrl.Instance.CircleOpen();
            }

            HttpRoutine.Create().Post(url, json, (ret) => {
                if (loadingCircle)
                {
                    CircleCtrl.Instance.CircleClose();
                }

                if (ret.result != UnityWebRequest.Result.Success)
                {
                    RequestFailed?.Invoke(ret);
                    return;
                }

                callBack?.Invoke(ret);
            });
        }

        public void Post(string url, string json = null, bool loadingCircle = false, Action<string> callBack = null)
        {
            PostArgs(url, json, loadingCircle, (args) => {
                callBack?.Invoke(args.downloadHandler.text);
            });
        }

        public UniTask<UnityWebRequest> PostArgsAsync(string url, string json = null, bool loadingCircle = false)
        {
            UniTaskCompletionSource<UnityWebRequest> task = new();
            PostArgs(url, json, loadingCircle, x => task.TrySetResult(x));
            return task.Task;
        }

        public UniTask<string> PostAsync(string url, string json = null, bool loadingCircle = false)
        {
            UniTaskCompletionSource<string> task = new();
            Post(url, json, loadingCircle, x => task.TrySetResult(x));
            return task.Task;
        }

    }
}