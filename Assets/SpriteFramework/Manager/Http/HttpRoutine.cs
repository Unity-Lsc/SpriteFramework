using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SpriteMain;
using System.Text;

namespace SpriteFramework
{
    /// <summary>
    /// Http发送数据的回调委托
    /// </summary>
    public delegate void HttpSendDataCallback(HttpCallBackArgs args);

    /// <summary>
    /// Http访问器
    /// </summary>
    public class HttpRoutine
    {

        /// <summary>
        /// Http请求回调
        /// </summary>
        private HttpSendDataCallback _callBack;

        /// <summary>
        /// Http请求回调数据
        /// </summary>
        private HttpCallBackArgs _callBackArgs;

        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy { get; private set; }

        /// <summary>
        /// 当前重试次数
        /// </summary>
        private int _curRetry = 0;

        private string _url;
        private string _json;

        /// <summary>
        /// 发送的数据
        /// </summary>
        private Dictionary<string, object> m_Dict;

        public HttpRoutine() {
            _callBackArgs = new HttpCallBackArgs();
            m_Dict = GameEntry.Pool.ClassObjectPool.Dequeue<Dictionary<string, object>>();
        }

        public static HttpRoutine Create() {
            return GameEntry.Pool.ClassObjectPool.Dequeue<HttpRoutine>();
        }

        /// <summary>
        /// 发送web数据
        /// </summary>
        public void Get(string url, HttpSendDataCallback callBack = null) {
            if (IsBusy) return;
            IsBusy = true;

            _url = url;
            _callBack = callBack;
            GetUrl(url);
        }

        public void Post(string url, string json = null, HttpSendDataCallback callBack = null) {
            if (IsBusy) return;
            IsBusy = true;

            _url = url;
            _callBack = callBack;
            _json = json;

            PostUrl(_url);
        }

        /// <summary>
        /// 请求服务器
        /// </summary>
        private IEnumerator Request(UnityWebRequest data) {
            data.timeout = 5;
            yield return data.SendWebRequest();
            if (data.result == UnityWebRequest.Result.Success) {
                IsBusy = false;
                _callBackArgs.HasError = false;
                _callBackArgs.Value = data.downloadHandler.text;
                _callBackArgs.Data = data.downloadHandler.data;
            } else {
                //报错了 进行重试
                if (_curRetry > 0) {
                    yield return new WaitForSeconds(MainEntry.ParamsSettings.HttpRetryInterval);
                }
                _curRetry++;
                if (_curRetry <= MainEntry.ParamsSettings.HttpRetry) {
                    switch (data.method) {
                        case UnityWebRequest.kHttpVerbGET:
                            GetUrl(_url);
                            break;
                        case UnityWebRequest.kHttpVerbPOST:
                            PostUrl(_url);
                            break;
                    }
                    yield break;
                }

                IsBusy = false;
                _callBackArgs.HasError = true;
                _callBackArgs.Value = data.error;
            }
            if (!string.IsNullOrWhiteSpace(_callBackArgs.Value)) {
                GameEntry.Log("WebAPI回调:{0}, ==>>{1}", LogCategory.Proto, _url, _callBackArgs.ToJson());
            }
            _callBack?.Invoke(_callBackArgs);

            _curRetry = 0;
            _url = null;
            if (m_Dict != null) {
                m_Dict.Clear();
                GameEntry.Pool.ClassObjectPool.Enqueue(m_Dict);
            }
            _callBackArgs.Data = null;
            data.Dispose();
            data = null;

            GameEntry.Pool.ClassObjectPool.Enqueue(this);
        }

        private void GetUrl(string url) {
            GameEntry.Log("Get请求:{0}, {1}次重试", LogCategory.Proto, _url, _curRetry);
            UnityWebRequest data = UnityWebRequest.Get(url);
            GameEntry.Instance.StartCoroutine(Request(data));
        }

        private void PostUrl(string url) {
            UnityWebRequest unityWeb = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            unityWeb.downloadHandler = new DownloadHandlerBuffer();
            if (!string.IsNullOrWhiteSpace(_json)) {
                if(MainEntry.ParamsSettings.PostIsEncrypt && _curRetry == 0) {
                    m_Dict["value"] = _json;
                    //web加密
                    m_Dict["deviceIdentifier"] = DeviceUtil.DeviceIdentifier;
                    m_Dict["deviceModel"] = DeviceUtil.DeviceModel;
                    //后续可更换成服务器时间
                    long t = (long)Time.unscaledTime;
                    m_Dict["sign"] = DataUtils.Md5(string.Format("{0}:{1}", t, DeviceUtil.DeviceIdentifier));
                    m_Dict["t"] = t;

                    _json = m_Dict.ToJson();
                }
                unityWeb.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(_json));

                if (!string.IsNullOrWhiteSpace(MainEntry.ParamsSettings.PostContentType))
                    unityWeb.SetRequestHeader("Content-Type", MainEntry.ParamsSettings.PostContentType);
            }
            GameEntry.Log("Post请求:{0},{1}次重试==>>{2}", LogCategory.Proto, _url, _curRetry, _json);
            GameEntry.Instance.StartCoroutine(Request(unityWeb));
        }

    }
}
