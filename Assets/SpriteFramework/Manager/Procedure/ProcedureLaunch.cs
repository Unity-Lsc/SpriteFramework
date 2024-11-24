namespace SpriteFramework
{
    /// <summary>
    /// 启动流程
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {

        public override void OnEnter() {
            base.OnEnter();
            //加载数据表
            GameEntry.DataTable.LoadDataTable();
            //切换进入主场景
            GameEntry.Scene.LoadSceneAsync("Main", () => {
                GameEntry.UI.OpenUIForm<UIMainCityForm>();
            });
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnLeave() {
            base.OnLeave();
        }

        public override void OnDestroy() {
            base.OnDestroy();
        }

    }
}
