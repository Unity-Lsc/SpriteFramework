using UnityEngine;

/// <summary>
/// 打印日志工具类
/// </summary>
public class Debugger
{

    /// <summary>
    /// 打印日志
    /// </summary>
    public static void Log(string message, LogCategory category = LogCategory.Normal, params object[] args) {
        switch (category) {
            default:
            case LogCategory.Normal:
#if DEBUG_MODE && DEBUG_LOG_NORMAL
                Debug.Log("[log]" + string.Format("<color=#5FE6FF>{0}</color>", args.Length == 0 ? message : string.Format(message, args)));
#endif
                break;
            case LogCategory.Necessary:
#if DEBUG_MODE && DEBUG_LOG_NECESSARY
                Debug.Log("[log]" + string.Format("<color=#ace44a>{0}</color>", args.Length == 0 ? message : string.Format(message, args)));
#endif
                break;
            case LogCategory.Proto:
#if DEBUG_MODE && DEBUG_LOG_PROTO
                Debug.Log("[log]" + string.Format("<color=#FFF299>{0}</color>", args.Length == 0 ? message : string.Format(message, args)));
#endif
                break;
        }
    }

    public static void Log(string message, params object[] args) {
#if DEBUG_MODE && DEBUG_LOG_NORMAL
        Debug.Log("[log]" + string.Format("<color=#5FE6FF>{0}</color>", args.Length == 0 ? message : string.Format(message, args)));
#endif
    }

    /// <summary>
    /// 打印错误日志
    /// </summary>
    public static void LogError(string message, params object[] args) {
#if DEBUG_MODE && DEBUG_LOG_ERROR
        Debug.LogError("[log]" + (args.Length == 0 ? message : string.Format(message, args)));
#endif
    }

}
