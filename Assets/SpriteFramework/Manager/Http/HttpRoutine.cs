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
        private HttpSendDataCallback m_CallBack;

        /// <summary>
        /// Http请求回调数据
        /// </summary>
        private HttpCallBackArgs m_CallBackArgs;

        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy { get; private set; }

        /// <summary>
        /// 当前重试次数
        /// </summary>
        private int m_CurrRetry = 0;

        private string m_Url;
        private string m_Json;

        /// <summary>
        /// 发送的数据
        /// </summary>
        private Dictionary<string, object> m_Dict;

        public HttpRoutine() {
            m_CallBackArgs = new HttpCallBackArgs();
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

            m_Url = url;
            m_CallBack = callBack;
            GetUrl(url);
        }

        public void Post(string url, string json = null, HttpSendDataCallback callBack = null) {
            if (IsBusy) return;
            IsBusy = true;

            m_Url = url;
            m_CallBack = callBack;
            m_Json = json;

            PostUrl(m_Url);
        }

        /// <summary>
        /// 请求服务器
        /// </summary>
        private IEnumerator Request(UnityWebRequest data) {
            data.timeout = 5;
            yield return data.SendWebRequest();
            if (data.result == UnityWebRequest.Result.Success) {
                IsBusy = false;
                m_CallBackArgs.HasError = false;
                m_CallBackArgs.Value = data.downloadHandler.text;
                m_CallBackArgs.Data = data.downloadHandler.data;
            } else {
                //报错了 进行重试
                if (m_CurrRetry > 0) {
                    yield return new WaitForSeconds(MainEntry.ParamsSettings.HttpRetryInterval);
                }
                m_CurrRetry++;
                if (m_CurrRetry <= MainEntry.ParamsSettings.HttpRetry) {
                    switch (data.method) {
                        case UnityWebRequest.kHttpVerbGET:
                            GetUrl(m_Url);
                            break;
                        case UnityWebRequest.kHttpVerbPOST:
                            PostUrl(m_Url);
                            break;
                    }
                    yield break;
                }

                IsBusy = false;
                m_CallBackArgs.HasError = true;
                m_CallBackArgs.Value = data.error;
            }
            if (!string.IsNullOrWhiteSpace(m_CallBackArgs.Value)) {
                GameEntry.Log("WebAPI回调:{0}, ==>>{1}", LogCategory.Proto, m_Url, m_CallBackArgs.ToJson());
            }
            m_CallBack?.Invoke(m_CallBackArgs);

            m_CurrRetry = 0;
            m_Url = null;
            if (m_Dict != null) {
                m_Dict.Clear();
                GameEntry.Pool.ClassObjectPool.Enqueue(m_Dict);
            }
            m_CallBackArgs.Data = null;
            data.Dispose();
            data = null;

            GameEntry.Pool.ClassObjectPool.Enqueue(this);
        }

        private void GetUrl(string url) {
            GameEntry.Log("Get请求:{0}, {1}次重试", LogCategory.Proto, m_Url, m_CurrRetry);
            UnityWebRequest data = UnityWebRequest.Get(url);
            GameEntry.Instance.StartCoroutine(Request(data));
        }

        private void PostUrl(string url) {
            UnityWebRequest unityWeb = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            unityWeb.downloadHandler = new DownloadHandlerBuffer();
            if (!string.IsNullOrWhiteSpace(m_Json)) {
                if(MainEntry.ParamsSettings.PostIsEncrypt && m_CurrRetry == 0) {
                    m_Dict["value"] = m_Json;
                    //web加密
                    m_Dict["deviceIdentifier"] = DeviceUtil.DeviceIdentifier;
                    m_Dict["deviceModel"] = DeviceUtil.DeviceModel;
                    //后续可更换成服务器时间
                    long t = (long)Time.unscaledTime;
                    m_Dict["sign"] = DataUtils.Md5(string.Format("{0}:{1}", t, DeviceUtil.DeviceIdentifier));
                    m_Dict["t"] = t;

                    m_Json = m_Dict.ToJson();
                }
                unityWeb.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(m_Json));

                if (!string.IsNullOrWhiteSpace(MainEntry.ParamsSettings.PostContentType))
                    unityWeb.SetRequestHeader("Content-Type", MainEntry.ParamsSettings.PostContentType);
            }
            GameEntry.Log("Post请求:{0},{1}次重试==>>{2}", LogCategory.Proto, m_Url, m_CurrRetry, m_Json);
            GameEntry.Instance.StartCoroutine(Request(unityWeb));
        }

    }
}
