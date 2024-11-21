using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// 状态机
    /// </summary>
    /// <typeparam name="T">拥有者</typeparam>
    public class Fsm<T> : FsmBase where T : class
    {

        /// <summary>
        /// 拥有者
        /// </summary>
        public T Owner { get; private set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        private FsmState<T> m_CurrState;

        /// <summary>
        /// 状态字典
        /// </summary>
        private Dictionary<sbyte, FsmState<T>> m_StateDict;

        /// <summary>
        /// 参数字典
        /// </summary>
        private Dictionary<string, VariableBase> m_ParamDict;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态数组</param>
        public Fsm(int fsmId, T owner, FsmState<T>[] states) : base(fsmId) {
            m_StateDict = new();
            m_ParamDict = new();
            Owner = owner;

            int len = states.Length;
            for (int i = 0; i < len; i++) {
                FsmState<T> state = states[i];
                if(states != null) {
                    state.CurrFsm = this;
                }
                m_StateDict[(sbyte)i] = state;
            }
            CurStateType = -1;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns></returns>
        public FsmState<T> GetState(sbyte stateType) {
            m_StateDict.TryGetValue(stateType, out FsmState<T> state);
            return state;
        }

        public void OnUpdate() {
            if(m_CurrState != null) {
                m_CurrState.OnUpdate();
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="newState">新状态的类型</param>
        public void ChangeState(sbyte newState) {
            if (CurStateType == newState) return;

            if(m_CurrState != null) {
                m_CurrState.OnLeave();
            }

            CurStateType = newState;
            m_CurrState = m_StateDict[CurStateType];

            //进入新的状态
            m_CurrState.OnEnter();
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <typeparam name="TData">泛型类型</typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetData<TData>(string key, TData value) {
            Variable<TData> item;
            if (m_ParamDict.TryGetValue(key, out VariableBase itemBase)) {
                item = itemBase as Variable<TData>;
            } else {
                //参数原来不存在
                item = new Variable<TData>();
            }
            item.Value = value;
            m_ParamDict[key] = item;
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TData GetData<TData>(string key) {
            if (m_ParamDict.TryGetValue(key, out VariableBase itemBase)) {
                Variable<TData> item = itemBase as Variable<TData>;
                return item.Value;
            }
            return default;
        }

        public override void ShutDown() {
            if(m_CurrState != null) {
                m_CurrState.OnLeave();
            }
            var enumerator = m_StateDict.GetEnumerator();
            while (enumerator.MoveNext()) {
                var state = enumerator.Current.Value;
                if (state != null) {
                    state.OnDestroy();
                }
            }
            m_StateDict.Clear();
            m_ParamDict.Clear();
        }
    }
}
