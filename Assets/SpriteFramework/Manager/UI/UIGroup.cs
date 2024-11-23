using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// UI分组
    /// </summary>
    [System.Serializable]
    public class UIGroup
    {

        /// <summary>
        /// 分组编号
        /// </summary>
        public byte Id;

        /// <summary>
        /// 基础排序
        /// </summary>
        public ushort BaseOrder;

        /// <summary>
        /// UI分组的Transform组件
        /// </summary>
        public Transform Tran;

    }
}
