using UnityEngine.Networking;
using SpriteFramework;

/// <summary>
/// 业务HTTP扩展服务
/// 用于承接框架层HttpManager抛出的失败事件,并决定业务层如何反馈
/// </summary>
public sealed class GameHttpService
{
    public GameHttpService()
    {
        GameEntry.Http.RequestFailed += OnRequestFailed;
    }

    /// <summary>
    /// 当前项目的默认失败反馈。
    /// 这里只是示例实现
    /// </summary>
    private void OnRequestFailed(UnityWebRequest request)
    {
        DialogForm.ShowFormByKey("Error404");
    }
}
