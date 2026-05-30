using SpriteFramework;

/// <summary>
/// 业务对话框服务
/// 负责统一收口Dialog相关表查询
/// </summary>
public sealed class GameDialogService
{
    /// <summary>
    /// 尝试按 key 获取对话框配置。
    /// </summary>
    public bool TryGetDialog(string key, out Sys_DialogEntity entity)
    {
        entity = null;
        Sys_DialogDBModel table = GameEntry.DataTable.Get<Sys_DialogDBModel>();
        return table != null && table.keyDic.TryGetValue(key, out entity);
    }

    /// <summary>
    /// 只获取对话框正文内容。
    /// 用于 Circle、Toast 这类只关心文本的场景。
    /// </summary>
    public bool TryGetDialogContent(string key, out string content)
    {
        content = null;
        if (!TryGetDialog(key, out Sys_DialogEntity entity))
        {
            return false;
        }

        content = entity.Content;
        return true;
    }

    /// <summary>
    /// 直接按 key 弹业务对话框。
    /// 这里是业务语义层，不属于框架层。
    /// </summary>
    public void ShowByKey(string key, DialogForm.DialogFormType type = DialogForm.DialogFormType.Affirm, System.Action okAction = null, System.Action cancelAction = null)
    {
        DialogForm.ShowFormByKey(key, type, okAction, cancelAction);
    }
}
