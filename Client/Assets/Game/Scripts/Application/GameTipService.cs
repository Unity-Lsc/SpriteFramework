using SpriteFramework;

/// <summary>
/// 业务Tip服务
/// </summary>
public sealed class GameTipService
{
    /// <summary>
    /// 尝试获取Tip配置
    /// </summary>
    public bool TryGetTip(string key, out Sys_TipEntity entity)
    {
        entity = null;
        Sys_TipDBModel table = GameEntry.DataTable.Get<Sys_TipDBModel>();
        return table != null && table.keyDic.TryGetValue(key, out entity);
    }

    /// <summary>
    /// 按业务key显示Tip
    /// </summary>
    public void ShowByKey(string key)
    {
        TipForm.ShowFormByKey(key);
    }
}
