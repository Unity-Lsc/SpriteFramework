using SpriteFramework;

/// <summary>
/// 登录流程
/// </summary>
public class ProcedureLogin : ProcedureBase
{
    public override void OnEnter(int lastState)
    {
        base.OnEnter(lastState);
        GameEntry.Procedure.ChangeState<ProcedureMain>();


    }
    internal override void OnDestroy()
    {
        base.OnDestroy();
    }
}