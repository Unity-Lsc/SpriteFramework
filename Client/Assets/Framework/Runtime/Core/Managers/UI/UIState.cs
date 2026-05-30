namespace SpriteFramework
{
    /// <summary>
    /// UI实例生命周期状态
    /// </summary>
    public enum UIState
    {
        None = 0,

        /// <summary>
        /// 已提交打开请求,资源正在加载或实例正在创建
        /// </summary>
        Loading,

        /// <summary>
        /// UI已经打开并处于显示状态
        /// </summary>
        Opened,

        /// <summary>
        /// UI已关闭显示,但实例仍缓存在对象池中
        /// </summary>
        Cached,

        /// <summary>
        /// UI正在关闭流程中
        /// </summary>
        Closing,

        /// <summary>
        /// UI已经销毁,打开列表和UI对象池中都不再存在该UI实例
        /// </summary>
        Destroyed,
    }
}
