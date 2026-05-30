using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// Procedure运行时组件
    /// 编辑器阶段会自动维护可用的流程列表和 首个进入的流程
    /// </summary>
    [DisallowMultipleComponent]
    public class ProcedureComponent : MonoBehaviour
    {
        /// <summary>
        /// 当前组件上可用的全部Procedure类型名
        /// 由编辑器脚本自动维护,运行时不做程序集扫描
        /// 这里保存 AssemblyQualifiedName,避免运行时自行遍历程序集解析类型
        /// </summary>
        [SerializeField, HideInInspector]
        private List<string> m_AvailableProcedureTypeNames = new();

        /// <summary>
        /// 入口Procedure类型名
        /// </summary>
        [SerializeField, HideInInspector]
        private string m_EntranceProcedureTypeName;

        /// <summary>
        /// 对外暴露只读入口类型名，便于调试或日志查看。
        /// </summary>
        public string EntranceProcedureTypeName => m_EntranceProcedureTypeName;

        /// <summary>
        /// Procedure 启动入口。
        /// 这里等待 GameEntry 和 Framework Managers 就绪，再初始化流程系统。
        /// </summary>
        private void Start()
        {
            InitializeProcedureManager();
        }

        /// <summary>
        /// 初始化 ProcedureManager 并启动入口流程。
        /// </summary>
        private void InitializeProcedureManager()
        {
            if (GameEntry.Procedure == null)
            {
                GameEntry.LogError(ELogCategory.Procedure, "ProcedureManager is null. Ensure GameEntry initializes managers in Awake.");
                return;
            }

            if (m_AvailableProcedureTypeNames == null || m_AvailableProcedureTypeNames.Count == 0)
            {
                GameEntry.LogError(ELogCategory.Procedure, "ProcedureComponent has no available procedures.");
                return;
            }

            List<ProcedureBase> procedures = new(m_AvailableProcedureTypeNames.Count);
            for (int i = 0; i < m_AvailableProcedureTypeNames.Count; i++)
            {
                string typeName = m_AvailableProcedureTypeNames[i];
                Type procedureType = Type.GetType(typeName, false);
                if (procedureType == null)
                {
                    GameEntry.LogError(ELogCategory.Procedure, $"Procedure type not found: {typeName}");
                    continue;
                }

                if (procedureType.IsAbstract || !typeof(ProcedureBase).IsAssignableFrom(procedureType))
                {
                    GameEntry.LogError(ELogCategory.Procedure, $"Invalid procedure type: {procedureType.FullName}");
                    continue;
                }

                if (Activator.CreateInstance(procedureType) is not ProcedureBase procedure)
                {
                    GameEntry.LogError(ELogCategory.Procedure, $"Procedure create failed: {procedureType.FullName}");
                    continue;
                }

                procedures.Add(procedure);
            }

            if (procedures.Count == 0)
            {
                GameEntry.LogError(ELogCategory.Procedure, "No valid procedures can be created.");
                return;
            }

            GameEntry.Procedure.Register(procedures.ToArray());

            Type entranceType = Type.GetType(m_EntranceProcedureTypeName, false);
            if (entranceType == null)
            {
                GameEntry.LogError(ELogCategory.Procedure, $"Entrance procedure type not found: {m_EntranceProcedureTypeName}");
                return;
            }

            GameEntry.Procedure.StartProcedure(entranceType);
        }

    }
}
