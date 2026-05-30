using SpriteFramework;
using Cysharp.Threading.Tasks;

/// <summary>
/// 游戏主流程
/// </summary>
public class ProcedureMain : ProcedureBase
{
    public override void OnEnter(int lastState)
    {
        base.OnEnter(lastState);
        GameApp.UI.OpenFormAsync<LoadingForm>().Forget();
        GameApp.Scene.LoadSceneByGroup(SceneGroupName.Main);
    }
    public override void OnLeave(int newState)
    {
        base.OnLeave(newState);
        //退出登录时, 清空业务数据
        GameEntry.Model.Clear();
    }
}
