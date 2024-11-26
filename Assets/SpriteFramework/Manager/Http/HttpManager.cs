using SpriteMain;
using System;

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
        private string _webAccountUrl;
        /// <summary>
        /// 测试账号服务器Url
        /// </summary>
        private string _testWebAccountUrl;
        /// <summary>
        /// 是否测试环境
        /// </summary>
        private bool m_IsTest;
        /// <summary>
        /// 真实账号服务器Url
        /// </summary>
        public string RealWebAccountUrl { get { return "http://" + RealIpAndPort + "/"; } }
        public string RealIpAndPort { get { return m_IsTest ? _testWebAccountUrl : _webAccountUrl; } }

        public HttpManager() {
            _webAccountUrl = MainEntry.ParamsSettings.WebAccountUrl;
            _testWebAccountUrl = MainEntry.ParamsSettings.TestWebAccountUrl;
            m_IsTest = MainEntry.ParamsSettings.IsTest;
        }

        public void GetArgs(string url, HttpSendDataCallback callback = null) {
            HttpRoutine.Create().Get(url, (HttpCallBackArgs ret) => {
                if (ret.HasError) {
                    GameEntry.Log("网络请求错误,错误信息:{0}", LogCategory.Proto, ret.Value);
                } else {
                    callback?.Invoke(ret);
                }
            });
        }

        public void Get(string url, Action<string> callback = null) {
            GetArgs(url, (args) => {
                if(args.Value.JsonCutApart("Status").ToInt() == 1) {
                    callback?.Invoke(args.Value.JsonCutApart("Content"));
                }
            });
        }

        public void PostArgs(string url, string json = null, HttpSendDataCallback callBack = null) {

            HttpRoutine.Create().Post(url, json, (HttpCallBackArgs ret) => {

                if (ret.HasError) {
                    GameEntry.Log("网络请求错误,错误信息:{0}", LogCategory.Proto, ret.Value);
                } else {
                    callBack?.Invoke(ret);
                }
            });
        }

        public void Post(string url, string json = null, Action<string> callBack = null) {
            PostArgs(url, json, (args) => {
                if (args.Value.JsonCutApart("Status").ToInt() == 1) callBack?.Invoke(args.Value.JsonCutApart("Content"));
            });
        }

    }
}
