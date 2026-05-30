using System.Linq;
using SpriteFramework;

/// <summary>
/// 启动流程
/// </summary>
public class ProcedureLaunch : ProcedureBase
{
    /// <summary>
    /// 当前项目启动时需要申请的权限列表。
    /// 这属于业务启动逻辑，不属于 Framework。
    /// </summary>
    private string[] permissions = new string[]
    {
        "android.permission.WRITE_EXTERNAL_STORAGE"
    };

    public override void OnEnter(int lastState)
    {
        base.OnEnter(lastState);

        //业务初始化放在入口流程执行
        GameApp.Init();

        //获取安卓权限
        permissions.ToList().ForEach(s =>
        {
            //if (!Permission.HasUserAuthorizedPermission(s)) Permission.RequestUserPermission(s);
        });

        GameEntry.Procedure.ChangeState<ProcedurePreload>();
    }
}