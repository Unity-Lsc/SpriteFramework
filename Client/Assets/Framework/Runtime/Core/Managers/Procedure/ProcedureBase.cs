namespace SpriteFramework
{
    /// <summary>
    /// 流程状态基类
    /// </summary>
    public class ProcedureBase : FsmState<ProcedureManager>
    {
        public override void OnEnter(int lastState)
        {
            base.OnEnter(lastState);
            GameEntry.Log(ELogCategory.Procedure, CurrFsm.GetState(CurrFsm.CurrStateType).ToString() + "==>OnEnter()");
        }

        public override void OnLeave(int newState)
        {
            base.OnLeave(newState);
            GameEntry.Log(ELogCategory.Procedure, CurrFsm.GetState(CurrFsm.CurrStateType).ToString() + "==>OnLeave()");
        }

        internal override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}