using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// 层级管理
    /// </summary>
    public class UILayer
    {
        /// <summary>
        /// 存储UI层级的集合（key->UIGroupId，Value->UIGroup的层级）
        /// </summary>
        private readonly Dictionary<byte, ushort> m_UILayerDict;

        public UILayer() {
            m_UILayerDict = new Dictionary<byte, ushort>();

            //初始化基础排序
            var groups = GameEntry.Instance.UIGroups;
            if (groups != null && groups.Length > 0) {
                for (int i = 0; i < groups.Length; i++) {
                    var group = groups[i];
                    m_UILayerDict[group.Id] = group.BaseOrder;
                }
            }
        }

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="formBase">窗口</param>
        /// <param name="isAdd">true:层级增加  false:层级减少</param>
        internal void SetSortingOrder(UIFormBase formBase, bool isAdd) {
            if (formBase.UIFormEntity.IsDisableUILayer == 1) return;
            if (!m_UILayerDict.ContainsKey(formBase.UIFormEntity.UIGroupId)) return;
            if (isAdd) {
                m_UILayerDict[formBase.UIFormEntity.UIGroupId] += 10;
            } else {
                m_UILayerDict[formBase.UIFormEntity.UIGroupId] -= 10;
            }
        }

        /// <summary>
        /// 获取窗体的层级
        /// </summary>
        /// <param name="formBase">要获取层级的窗体</param>
        internal int GetCurSortingOrder(UIFormBase formBase) {
            return m_UILayerDict[formBase.UIFormEntity.UIGroupId];
        }

        public void Dispose() {
            m_UILayerDict.Clear();
        }

    }
}
