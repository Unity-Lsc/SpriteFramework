using SpriteFramework;
using System.Collections;
using UnityEngine;

/// <summary>
/// 游戏入口
/// </summary>
public class GameEntry : UnitySingleton<GameEntry>
{
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init() {

    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    public IEnumerator EnterGame() {
        Debugger.Log("进入游戏");

        //测试同步加载资源
        TextAsset t = ResMgr.Instance.LoadDataTable<TextAsset>("fruit");
        Debug.Log(t.text);
        //end

        //测试异步加载资源
        var handle = ResMgr.Instance.LoadDataTableAsync<TextAsset>("fragment");
        yield return handle;
        t = handle.AssetObject as TextAsset;
        handle.Dispose();
        Debug.Log(t.text);
        //end

        yield return SceneMgr.Instance.LoadScene("Main");
    }

}
