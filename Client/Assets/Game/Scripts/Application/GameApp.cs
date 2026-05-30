using SpriteFramework;
using System;

/// <summary>
/// 业务层总入口
/// 这一层负责把“业务配置”和“框架能力”组装起来，避免框架直接碰业务表
/// </summary>
public static class GameApp
{
    /// <summary>
    /// 是否已经完成业务层装配
    /// 防止重复初始化导致事件重复注册或服务重复创建
    /// </summary>
    public static bool IsInitialized { get; private set; }

    /// <summary>
    /// 业务UI服务
    /// 负责把UI表配置翻译成框架UIManager可识别的参数
    /// </summary>
    public static GameUIService UI { get; private set; }

    /// <summary>
    /// 业务音频服务
    /// </summary>
    public static GameAudioService Audio { get; private set; }

    /// <summary>
    /// 业务场景服务
    /// </summary>
    public static GameSceneService Scene { get; private set; }

    /// <summary>
    /// 业务对话框服务
    /// </summary>
    public static GameDialogService Dialog { get; private set; }

    /// <summary>
    /// 业务提示服务
    /// </summary>
    public static GameTipService Tip { get; private set; }

    /// <summary>
    /// 业务 HTTP 扩展服务
    /// 用来承接框架 HTTP 失败后的业务反馈逻辑
    /// </summary>
    public static GameHttpService Http { get; private set; }

    /// <summary>
    /// 业务初始化入口
    /// 时机放在GameEntry初始化完成之后
    /// </summary>
    public static void Init()
    {
        if (IsInitialized) return;

        if (GameEntry.DataTable == null)
        {
            throw new InvalidOperationException("GameEntry.DataTable is null, initialize GameApp after GameEntry.Start.");
        }

        //注册所有的配置表
        GameDataTable.CreateAllDataTable(GameEntry.DataTable);

        UI = new GameUIService();
        Audio = new GameAudioService();
        Scene = new GameSceneService();
        Dialog = new GameDialogService();
        Tip = new GameTipService();
        Http = new GameHttpService();

        IsInitialized = true;
    }
}
