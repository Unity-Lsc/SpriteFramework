namespace SpriteFramework
{
    /// <summary>
    /// 流程基类
    /// </summary>
    public class ProcedureBase : FsmState<ProcedureManager>
    {

        public override void OnEnter() {
            base.OnEnter();
            GameEntry.Log(CurrFsm.GetState(CurrFsm.CurStateType).ToString() + "======OnEnter()");
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnLeave() {
            base.OnLeave();
            GameEntry.Log(CurrFsm.GetState(CurrFsm.CurStateType).ToString() + "======OnLeave()");
        }

        public override void OnDestroy() {
            base.OnDestroy();
            GameEntry.Log(CurrFsm.GetState(CurrFsm.CurStateType).ToString() + "======OnDestroy()");
        }

    }
}
