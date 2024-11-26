using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteFramework
{
    /// <summary>
    /// 游戏框架的启动入口
    /// </summary>
    public class GameEntry : MonoBehaviour
    {
        [Header("UI摄像机")]
        public Camera UICamera;

        [Header("根画布")]
        public Canvas UIRootCanvas;

        [Header("根画布的缩放")]
        public CanvasScaler UIRootCanvasScaler;
        public RectTransform UIRootRectTransform { get; private set; }

        [Header("UI分组")]
        public UIGroup[] UIGroups;

        [Header("当前语言（要和本地化表的语言字段 一致）")]
        [SerializeField]
        private SpriteLanguage _curLanguage;

        public static SpriteLanguage CurLanguage;

        //管理器属性
        public static EventManager Event { get; private set; }
        public static TimeManager Time { get; private set; }
        public static ResManager Resource { get; private set; }
        public static SpriteSceneManager Scene { get; private set; }
        public static PlayerPrefsManager PlayerPrefs { get; private set; }
        public static AudioManager Audio { get; private set; }
        public static DataTableManager DataTable { get; private set; }
        public static HttpManager Http { get; private set; }
        public static PoolManager Pool { get; private set; }
        public static LocalizationManager Localization { get; private set; }
        public static FsmManager Fsm { get; private set; }
        public static ProcedureManager Procedure { get; private set; }
        public static SocketManager Socket { get; private set; }
        public static UIManager UI { get; private set; }
        public static LuaManager Lua { get; private set; }

        public static GameEntry Instance { get; private set; }

        private void Awake() {
            Instance = this;
            CurLanguage = _curLanguage;
            UIRootRectTransform = UIRootCanvasScaler.GetComponent<RectTransform>();
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
            PlayerPrefs = new PlayerPrefsManager();
            Audio = new AudioManager();
            DataTable = new DataTableManager();
            Http = new HttpManager();
            Pool = new PoolManager();
            Localization = new LocalizationManager();
            Fsm = new FsmManager();
            Procedure = new ProcedureManager();
            Socket = new SocketManager();
            UI = new UIManager();
            Lua = new LuaManager();

            //在Init中, 模块之间可互相调用
            Audio.Init();
            Procedure.Init();

            EnterGame();
        }

        /// <summary>
        /// 进入游戏
        /// </summary>
        private void EnterGame() {
            Log("进入游戏");
            Procedure.ChangeState(ProcedureState.Launch);
            //StartCoroutine(TestGame());
        }

        private void Update() {
            //模块的OnUpdate,统一在这里调用
            Time.OnUpdate();
            Audio.OnUpdate();
            Pool.OnUpdate();
            Procedure.OnUpdate();
            Socket.OnUpdate();
            UI.OnUpdate();
            Scene.OnUpdate();
        }

        //private IEnumerator TestGame() {

        //    yield return Scene.LoadScene("Main", ()=> {
        //        UI.OpenUIForm<UIMainCityForm>();
        //    });
        //}

        #region 打印日志

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="category">日志类别</param>
        /// <param name="args">日志内容的参数</param>
        public static void Log(object message, LogCategory category = LogCategory.Normal, params object[] args) {
            switch (category) {
                default:
                case LogCategory.Normal:
#if DEBUG_MODE && DEBUG_LOG_NORMAL
                    Debug.Log("[log]" + string.Format("<color=#5FE6FF>{0}</color>", args.Length == 0 ? message : string.Format(message.ToString(), args)));
#endif
                    break;
                case LogCategory.Necessary:
#if DEBUG_MODE && DEBUG_LOG_NECESSARY
                    Debug.Log("[log]" + string.Format("<color=#ace44a>{0}</color>", args.Length == 0 ? message : string.Format(message.ToString(), args)));
#endif
                    break;
                case LogCategory.Proto:
#if DEBUG_MODE && DEBUG_LOG_PROTO
                    Debug.Log("[log]" + string.Format("<color=#FFF299>{0}</color>", args.Length == 0 ? message : string.Format(message.ToString(), args)));
#endif
                    break;
            }
        }

        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="args">日志内容的参数</param>
        public static void Log(object message, params object[] args) {
#if DEBUG_MODE && DEBUG_LOG_NORMAL
            Debug.Log("[log]" + string.Format("<color=#5FE6FF>{0}</color>", args.Length == 0 ? message : string.Format(message.ToString(), args)));
#endif
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        public static void LogWarning(object message, params object[] args) {
#if DEBUG_MODE && DEBUG_LOG_WARNING
            Debug.LogWarning("[log]" + (args.Length == 0 ? message : string.Format(message.ToString(), args)));
#endif
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        public static void LogError(object message, params object[] args) {
#if DEBUG_MODE && DEBUG_LOG_ERROR
            Debug.LogError("[log]" + (args.Length == 0 ? message : string.Format(message.ToString(), args)));
#endif
        }

        #endregion 打印日志end

        private void OnDestroy() {
            Event.Dispose();
            PlayerPrefs.Dispose();
            Audio.Dispose();
            DataTable.Dispose();
            Pool.Dispose();
            Fsm.Dispose();
            Socket.Dispose();
            UI.Dispose();
        }

    }

}