namespace SpriteFramework
{
    /// <summary>
    /// 打开UI界面时的参数配置
    /// </summary>
    public sealed class UIFormOpenOptions
    {
        /// <summary>
        /// 目标UI组编号
        /// </summary>
        public byte UIGroupId;

        /// <summary>
        /// 是否允许多实例同时打开
        /// </summary>
        public bool AllowMultiInstance;

        /// <summary>
        /// 是否跳过框架层级管理
        /// true时不修改Canvas sortingOrder
        /// </summary>
        public bool DisableLayer;

        /// <summary>
        /// 是否常驻对象池。
        /// true 表示关闭后不会被对象池过期清理
        /// </summary>
        public bool IsResident;

        /// <summary>
        /// 显示模式。
        /// Normal：普通叠加显示。
        /// ReverseChange：打开新页面时隐藏上一个反切页面，关闭时恢复。
        /// </summary>
        public EUIFormShowMode ShowMode = EUIFormShowMode.Normal;

        /// <summary>
        /// 创建一份快照,避免外部在打开流程中修改options导致运行时状态变化
        /// </summary>
        public UIFormOpenOptions Clone()
        {
            return new UIFormOpenOptions
            {
                UIGroupId = UIGroupId,
                AllowMultiInstance = AllowMultiInstance,
                DisableLayer = DisableLayer,
                IsResident = IsResident,
                ShowMode = ShowMode,
            };
        }

    }
}
