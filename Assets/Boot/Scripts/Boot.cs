using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 项目启动
/// </summary>
public class Boot : MonoBehaviour
{

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(BootStartUp());
    }

    /// <summary>
    /// 项目启动入口
    /// </summary>
    IEnumerator BootStartUp() {

        //检查热更新
        yield return CheckHotUpdate();
        //end

        //框架初始化
        yield return InitFramework();
        //end

        //进入游戏
        //end

        yield break;
    }

    /// <summary>
    /// 检查热更新
    /// </summary>
    IEnumerator CheckHotUpdate() {
        yield break;
    }

    /// <summary>
    /// 框架初始化
    /// </summary>
    /// <returns></returns>
    IEnumerator InitFramework() {
        yield break;
    }

}
