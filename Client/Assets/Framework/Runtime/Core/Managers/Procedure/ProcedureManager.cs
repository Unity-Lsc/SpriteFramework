using System;
using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// 流程管理器
    /// </summary>
    public class ProcedureManager
    {

        /// <summary>
        /// 当前流程状态机
        /// </summary>
        public Fsm<ProcedureManager> CurrFsm { get; private set; }

        // <summary>
        /// Procedure 类型到状态索引的映射
        /// 通过类型切换状态，替代固定业务枚举
        /// </summary>
        private readonly Dictionary<Type, sbyte> m_ProcedureTypeIndexMap = new();

        /// <summary>
        /// 注册业务模块的所有流程
        /// </summary>
        public void Register(params ProcedureBase[] procedures)
        {
            if(procedures == null || procedures.Length == 0)
            {
                GameEntry.LogError(ELogCategory.Procedure, "ProcedureManager.Initialize procedures is null or empty");
            }

            m_ProcedureTypeIndexMap.Clear();
            FsmState<ProcedureManager>[] states = new FsmState<ProcedureManager>[procedures.Length];
            for (sbyte i = 0; i < procedures.Length; i++)
            {
                ProcedureBase procedure = procedures[i];
                states[i] = procedure;
                m_ProcedureTypeIndexMap[procedure.GetType()] = i;
            }

            CurrFsm = GameEntry.Fsm.Create(this, states);
        }

        /// <summary>
        /// 启动入口流程（泛型版）。
        /// </summary>
        public void StartProcedure<T>() where T : ProcedureBase
        {
            ChangeState<T>();
        }

        /// <summary>
        /// 启动入口流程（Type 版）。
        /// ProcedureComponent 在运行时使用这个接口。
        /// </summary>
        public void StartProcedure(Type procedureType)
        {
            ChangeState(procedureType);
        }

        /// <summary>
        /// 按流程类型切换状态（泛型版）。
        /// </summary>
        public void ChangeState<T>() where T : ProcedureBase
        {
            ChangeState(typeof(T));
        }

        /// <summary>
        /// 按流程类型切换状态（Type 版）。
        /// </summary>
        public void ChangeState(Type procedureType)
        {
            EnsureInitialized();

            sbyte stateType = GetStateType(procedureType);
            CurrFsm.ChangeState(stateType);
        }

        /// <summary>
        /// 给指定流程传递参数（泛型版）。
        /// </summary>
        public void SetInfoList<T>(List<object> infoList) where T : ProcedureBase
        {
            SetInfoList(typeof(T), infoList);
        }

        /// <summary>
        /// 给指定流程传递参数（Type 版）。
        /// </summary>
        public void SetInfoList(Type procedureType, List<object> infoList)
        {
            EnsureInitialized();

            sbyte stateType = GetStateType(procedureType);
            FsmState<ProcedureManager> fsmState = CurrFsm.GetState(stateType);
            fsmState?.SetInfoList(infoList);
        }

        internal void OnUpdate()
        {
            CurrFsm.OnUpdate();
        }

        /// <summary>
        /// 根据 Procedure 类型获取对应的状态索引。
        /// </summary>
        private sbyte GetStateType(Type procedureType)
        {
            if (procedureType == null)
            {
                throw new ArgumentNullException(nameof(procedureType));
            }

            if (m_ProcedureTypeIndexMap.TryGetValue(procedureType, out sbyte stateType))
            {
                return stateType;
            }

            GameEntry.LogError(ELogCategory.Procedure, "ProcedureManager.Initialize procedures is null or empty");
            return -1;
        }

        private void EnsureInitialized()
        {
            if (CurrFsm == null)
            {
                throw new Exception("ProcedureManager is not initialized.");
            }
        }

    }
}