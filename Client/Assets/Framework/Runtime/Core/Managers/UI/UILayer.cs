using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// UI层级管理
    /// </summary>
    public class UILayer
    {
        private readonly Dictionary<byte, ushort> m_UILayerDic;

        public UILayer()
        {
            m_UILayerDic = new Dictionary<byte, ushort>();

            //初始化基础排序
            for (int i = 0; i < GameEntry.Instance.UIGroups.Length; i++)
            {
                UIGroup group = GameEntry.Instance.UIGroups[i];
                m_UILayerDic[group.Id] = group.BaseOrder;
            }
        }

        /// <summary>
        /// 根据 UI 运行时信息调整组内当前层级游标
        /// </summary>
        /// <param name="Sys_UIFormEntity">窗口</param>
        /// <param name="isAdd">true:增加  false:减少</param>
        internal void SetSortingOrder(UIFormRuntimeInfo runtimeInfo, bool isAdd)
        {
            if (runtimeInfo.DisableLayer) return;
            if (m_UILayerDic.ContainsKey(runtimeInfo.UIGroupId) == false) return;

            if (isAdd)
            {
                m_UILayerDic[runtimeInfo.UIGroupId] += 10;
            }
            else
            {
                m_UILayerDic[runtimeInfo.UIGroupId] -= 10;
            }
        }

        /// <summary>
        /// 获取当前组的可用层级值
        /// </summary>
        internal int GetCurrSortingOrder(UIFormRuntimeInfo runtimeInfo)
        {
            if (runtimeInfo.DisableLayer) return 0;

            return m_UILayerDic[runtimeInfo.UIGroupId];
        }

        internal void Dispose()
        {
            m_UILayerDic.Clear();
        }

    }
}