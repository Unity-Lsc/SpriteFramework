using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// 状态机管理器
    /// </summary>
    public class FsmManager
    {
        /// <summary>
        /// 存储状态机的集合
        /// </summary>
        private Dictionary<int, FsmBase> _fsmDict;

        /// <summary>
        /// 状态机的临时编号
        /// </summary>
        private int _fsmTempId = 0;

        public FsmManager() {
            _fsmDict = new();
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态数组</param>
        public Fsm<T> Create<T>(T owner, FsmState<T>[] states) where T : class {
            return Create<T>(_fsmTempId++, owner, states);
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态数组</param>
        public Fsm<T> Create<T>(int fsmId, T owner, FsmState<T>[] states) where T : class {
            Fsm<T> fsm = new Fsm<T>(fsmId, owner, states);
            _fsmDict[fsmId] = fsm;
            return fsm;
        }

        /// <summary>
        /// 销毁状态机
        /// </summary>
        public void DestroyFsm(int fsmId) {
            if(_fsmDict.TryGetValue(fsmId, out FsmBase fsm)) {
                fsm.ShutDown();
                _fsmDict.Remove(fsmId);
            }
        }

        public void Dispose() {
            var enumerator = _fsmDict.GetEnumerator();
            while (enumerator.MoveNext()) {
                enumerator.Current.Value.ShutDown();
            }
            _fsmDict.Clear();
        }

    }
}
