namespace SpriteFramework
{

    /// <summary>
    /// 流程状态
    /// </summary>
    public enum ProcedureState
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Launch = 0,
        /// <summary>
        /// 检查更新
        /// </summary>
        CheckVersion,
        /// <summary>
        /// 预加载
        /// </summary>
        Preload,
        /// <summary>
        /// 登录
        /// </summary>
        Login,
        /// <summary>
        /// 游戏主流程
        /// </summary>
        Main,
    }

    /// <summary>
    /// 流程管理器
    /// </summary>
    public class ProcedureManager
    {
        /// <summary>
        /// 当前流程的状态机
        /// </summary>
        private Fsm<ProcedureManager> m_CurFsm;
        /// <summary>
        /// 当前流程的状态机（属性）
        /// </summary>
        public Fsm<ProcedureManager> CurFsm {
            get {
                return m_CurFsm;
            }
        }

        /// <summary>
        /// 当前流程状态 的类型
        /// </summary>
        public ProcedureState CurProcedureStateType {
            get {
                if(m_CurFsm == null) {
                    return ProcedureState.Launch;
                }
                return (ProcedureState)m_CurFsm.CurStateType;
            }
        }

        /// <summary>
        /// 当前流程的状态
        /// </summary>
        public FsmState<ProcedureManager> CurProcedureState {
            get {
                return m_CurFsm.GetState(m_CurFsm.CurStateType);
            }
        }

        public void Init() {
            FsmState<ProcedureManager>[] states = new FsmState<ProcedureManager>[5];
            states[0] = new ProcedureLaunch();
            states[1] = new ProcedureCheckVersion();
            states[2] = new ProcedurePreload();
            states[3] = new ProcedureLogin();
            states[4] = new ProcedureMain();

            m_CurFsm = GameEntry.Fsm.Create(this, states);
        }

        /// <summary>
        /// 切换流程状态
        /// </summary>
        public void ChangeState(ProcedureState newState) {
            m_CurFsm.ChangeState((sbyte)newState);
        }

        public void OnUpdate() {
            m_CurFsm.OnUpdate();
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <typeparam name="TData">泛型类型</typeparam>
        public void SetData<TData>(string key, TData value) {
            m_CurFsm.SetData<TData>(key, value);
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        public TData GetData<TData>(string key) {
            return m_CurFsm.GetData<TData>(key);
        }

    }
}
