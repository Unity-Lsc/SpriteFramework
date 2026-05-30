namespace SpriteFramework
{
    /// <summary>
    /// UI运行时信息
    /// </summary>
    public sealed class UIFormRuntimeInfo
    {
        /// <summary>
        /// 本次打开请求的唯一序列号。
        /// </summary>
        public int SerialId;

        /// <summary>
        /// UI预制体完整资源路径
        /// </summary>
        public string AssetFullPath;

        /// <summary>
        /// 所属 UIGroup Id
        /// </summary>
        public byte UIGroupId;

        /// <summary>
        /// 是否允许多开
        /// </summary>
        public bool AllowMultiInstance;

        /// <summary>
        /// 是否禁用层级控制
        /// </summary>
        public bool DisableLayer;

        /// <summary>
        /// 是否常驻
        /// </summary>
        public bool IsResident;

        /// <summary>
        /// 显示模式
        /// </summary>
        public EUIFormShowMode ShowMode;

        /// <summary>
        /// 本次打开携带的业务数据
        /// </summary>
        public object UserData;
    }
}
