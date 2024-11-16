using System.Collections;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 游戏框架的启动入口
    /// </summary>
    public class GameEntry : MonoBehaviour
    {

        //管理器属性
        public static EventManager Event { get; private set; }
        public static TimeManager Time { get; private set; }
        public static ResManager Resource { get; private set; }
        public static SpriteSceneManager Scene { get; private set; }

        public static GameEntry Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init() {
            //在new的构造函数中, 构造自身, 模块之间不可互相调用, 因为其他模块可能是null
            Event = new EventManager();
            Time = new TimeManager();
            Resource = new ResManager();
            Scene = new SpriteSceneManager();

            //在Init中, 模块之间可互相调用
            //XX.Init();

            EnterGame();
        }

        /// <summary>
        /// 进入游戏
        /// </summary>
        private void EnterGame() {
            Log("进入游戏");

            StartCoroutine(TestGame());
        }

        private void Update() {
            //模块的OnUpdate,统一在这里调用
            Time.OnUpdate();
        }

        private IEnumerator TestGame() {
            //测试同步加载资源
            TextAsset t = Resource.LoadDataTable<TextAsset>("fruit");
            Debug.Log(t.text);
            //end

            //测试异步加载资源
            var handle = Resource.LoadDataTableAsync<TextAsset>("fragment");
            yield return handle;
            t = handle.AssetObject as TextAsset;
            handle.Dispose();
            Debug.Log(t.text);
            //end

            yield return Scene.LoadScene("Main");
        }

        #region 打印日志

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="category">日志类别</param>
        /// <param name="args">日志内容的参数</param>
        public static void Log(string message, LogCategory category = LogCategory.Normal, params object[] args) {
            switch (category) {
                default:
                case LogCategory.Normal:
#if DEBUG_MODE && DEBUG_LOG_NORMAL
                    Debug.Log("[log]" + string.Format("<color=#5FE6FF>{0}</color>", args.Length == 0 ? message : string.Format(message, args)));
#endif
                    break;
                case LogCategory.Necessary:
#if DEBUG_MODE && DEBUG_LOG_NECESSARY
                    Debug.Log("[log]" + string.Format("<color=#ace44a>{0}</color>", args.Length == 0 ? message : string.Format(message, args)));
#endif
                    break;
                case LogCategory.Proto:
#if DEBUG_MODE && DEBUG_LOG_PROTO
                    Debug.Log("[log]" + string.Format("<color=#FFF299>{0}</color>", args.Length == 0 ? message : string.Format(message, args)));
#endif
                    break;
            }
        }

        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="args">日志内容的参数</param>
        public static void Log(string message, params object[] args) {
#if DEBUG_MODE && DEBUG_LOG_NORMAL
            Debug.Log("[log]" + string.Format("<color=#5FE6FF>{0}</color>", args.Length == 0 ? message : string.Format(message, args)));
#endif
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        public static void LogWarning(string message, params object[] args) {
#if DEBUG_MODE && DEBUG_LOG_WARNING
            Debug.LogWarning("[log]" + (args.Length == 0 ? message : string.Format(message, args)));
#endif
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        public static void LogError(string message, params object[] args) {
#if DEBUG_MODE && DEBUG_LOG_ERROR
            Debug.LogError("[log]" + (args.Length == 0 ? message : string.Format(message, args)));
#endif
        }

        #endregion 打印日志end

        private void OnDestroy() {
            Event.Dispose();
        }

    }

}