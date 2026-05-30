using Cysharp.Threading.Tasks;
using SpriteFramework;
using System;
using System.Collections.Generic;

/// <summary>
/// 业务场景服务
/// 把场景组这类业务概念翻译成框架场景管理器能直接执行的场景路径列表
/// </summary>
public sealed class GameSceneService
{
    /// <summary>
    /// 异步按业务场景组加载场景
    /// </summary>
    public UniTask LoadSceneByGroupAsync(string groupName, int sceneLoadCount = -1)
    {
        List<string> sceneAssetPaths = GetSceneAssetPaths(groupName, sceneLoadCount);
        return GameEntry.Scene.LoadSceneAsync(sceneAssetPaths, groupName);
    }

    /// <summary>
    /// 按业务场景组加载场景,并支持回调
    /// </summary>
    public void LoadSceneByGroup(string groupName, int sceneLoadCount = -1, Action onComplete = null)
    {
        List<string> sceneAssetPaths = GetSceneAssetPaths(groupName, sceneLoadCount);
        GameEntry.Scene.LoadSceneAction(sceneAssetPaths, groupName, onComplete);
    }

    /// <summary>
    /// 从业务场景表中取出对应组的场景资源路径列表
    /// </summary>
    private static List<string> GetSceneAssetPaths(string groupName, int sceneLoadCount)
    {
        List<Sys_SceneEntity> entities = GameEntry.DataTable.Get<Sys_SceneDBModel>().GetListByGroupName(groupName, sceneLoadCount);
        List<string> sceneAssetPaths = new List<string>(entities.Count);
        for (int i = 0; i < entities.Count; i++)
        {
            sceneAssetPaths.Add(entities[i].AssetFullPath);
        }
        return sceneAssetPaths;
    }
}
