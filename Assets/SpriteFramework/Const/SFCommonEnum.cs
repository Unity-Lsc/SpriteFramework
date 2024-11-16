
/// <summary>
/// 日志分类
/// </summary>
public enum LogCategory
{
    /// <summary>
    /// 普通日志
    /// </summary>
    Normal,

    /// <summary>
    /// 必要日志
    /// </summary>
    Necessary,

    /// <summary>
    /// 通讯日志
    /// </summary>
    Proto,
}

/// <summary>
/// Loading类型
/// </summary>
public enum LoadingType
{
    /// <summary>
    /// 检查更新
    /// </summary>
    CheckVersion = 0,

    /// <summary>
    /// 切换场景
    /// </summary>
    ChangeScene = 1
}
