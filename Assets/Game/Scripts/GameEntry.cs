using SpriteFramework;
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
    public void EnterGame() {
        Debug.Log("进入游戏...");
        SceneMgr.Instance.EnterScene("Main");
    }

}
