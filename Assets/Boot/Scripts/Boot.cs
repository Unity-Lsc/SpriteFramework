using SpriteFramework;
using System.Collections;
using UnityEngine;
using YooAsset;

/// <summary>
/// 项目启动
/// </summary>
public class Boot : UnitySingleton<Boot>
{

    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    public string RemoteURL = "http://127.0.0.1:6080";

    public override void Awake() {
        base.Awake();

        Application.targetFrameRate = 60;
        Application.runInBackground = true;

        StartCoroutine(BootStartUp());
    }

    /// <summary>
    /// 项目启动入口
    /// </summary>
    IEnumerator BootStartUp() {

        //初始化YooAsset
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);
        //end

        //检查热更新
        yield return CheckHotUpdate();
        //end

        //框架初始化
        yield return InitFramework();
        //end

        //进入游戏
        yield return GameEntry.Instance.EnterGame();
        //end
    }

    /// <summary>
    /// 检查热更新
    /// </summary>
    IEnumerator CheckHotUpdate() {
        YooAssetHotUpdate.Instance.Init(PlayMode, RemoteURL);
        yield return YooAssetHotUpdate.Instance.GameHotUpdate();
    }

    /// <summary>
    /// 框架初始化
    /// </summary>
    IEnumerator InitFramework() {
        gameObject.AddComponent<ResMgr>().Init();
        gameObject.AddComponent<SceneMgr>().Init();
        gameObject.AddComponent<GameEntry>().Init();
        yield break;
    }

}
