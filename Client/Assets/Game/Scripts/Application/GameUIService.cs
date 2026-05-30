using Cysharp.Threading.Tasks;
using SpriteFramework;
using System;

/// <summary>
/// 业务层UI服务
/// 这一层负责把UI表配置(配置表,Json或者ScriptableObject等配置数据)翻译成框架UIManager可识别的参数
/// </summary>
public sealed class GameUIService
{
    /// <summary>
    /// 当前项目里默认UI组
    /// </summary>
    public const byte DefaultUIGroupId = 2;

    /// <summary>
    /// 按类型打开UI
    /// 返回序列号,可用于加载中取消或后续关闭
    /// </summary>
    public int OpenForm<T>(object userData = null) where T : UIFormBase
    {
        string formName = typeof(T).Name;
        Sys_UIFormEntity entity = GetConfig(formName);
        return GameEntry.UI.OpenUIForm(entity.AssetFullPath, CreateOptions(entity), userData);
    }

    /// <summary>
    /// 按类型打开UI,并使用单次回调
    /// 业务层无需订阅全局事件
    /// </summary>
    public int OpenForm<T>(Action<T> onSuccess, Action<string> onFailed = null, object userData = null) where T : UIFormBase
    {
        string formName = typeof(T).Name;
        Sys_UIFormEntity entity = GetConfig(formName);

        return GameEntry.UI.OpenUIForm<T>(entity.AssetFullPath, CreateOptions(entity), onSuccess, onFailed, userData);
    }

    /// <summary>
    /// 按业务名打开UI
    /// </summary>
    public int OpenForm(string formName, object userData = null)
    {
        Sys_UIFormEntity entity = GetConfig(formName);
        return GameEntry.UI.OpenUIForm(entity.AssetFullPath, CreateOptions(entity), userData);
    }

    /// <summary>
    /// async/await方式按类型打开UI
    /// </summary>
    public async UniTask<T> OpenFormAsync<T>(object userData = null) where T : UIFormBase
    {
        string formName = typeof(T).Name;
        Sys_UIFormEntity entity = GetConfig(formName);
        return await GameEntry.UI.OpenUIFormAsync<T>(entity.AssetFullPath, CreateOptions(entity), userData);
    }

    /// <summary>
    /// 按类型关闭UI
    /// </summary>
    public void CloseForm<T>() where T : UIFormBase
    {
        string formName = typeof(T).Name;
        Sys_UIFormEntity entity = GetConfig(formName);
        GameEntry.UI.CloseUIForm(entity.AssetFullPath);
    }

    /// <summary>
    /// 按序列号关闭UI
    /// </summary>
    public void CloseForm(int serialId)
    {
        GameEntry.UI.CloseUIForm(serialId);
    }

    /// <summary>
    /// 按业务名关闭UI
    /// </summary>
    public void CloseForm(string formName)
    {
        Sys_UIFormEntity entity = GetConfig(formName);
        GameEntry.UI.CloseUIForm(entity.AssetFullPath);
    }

    /// <summary>
    /// 查询UI
    /// </summary>
    public T GetForm<T>() where T : UIFormBase
    {
        string formName = typeof(T).Name;
        Sys_UIFormEntity entity = GetConfig(formName);
        return GameEntry.UI.GetUIForm<T>(entity.AssetFullPath);
    }

    /// <summary>
    /// 关闭全部界面
    /// </summary>
    public void CloseAllForms()
    {
        GameEntry.UI.CloseAllUIForms();
    }

    /// <summary>
    /// 按组关闭界面
    /// 组的含义仍由业务配置决定,框架只做执行
    /// </summary>
    public void CloseFormsByGroup(byte groupId)
    {
        GameEntry.UI.CloseUIFormsByGroup(groupId);
    }

    /// <summary>
    /// 关闭默认组界面
    /// </summary>
    public void CloseDefaultForms()
    {
        CloseFormsByGroup(DefaultUIGroupId);
    }

    /// <summary>
    /// 从业务配置中读取界面配置
    /// </summary>
    private static Sys_UIFormEntity GetConfig(string formName)
    {
        Sys_UIFormDBModel table = GameEntry.DataTable.Get<Sys_UIFormDBModel>();
        Sys_UIFormEntity entity = table.GetEntityByName(formName);
        if (entity == null)
        {
            throw new System.Exception($"UI config not found. formName={formName}");
        }
        return entity;
    }

    /// <summary>
    /// 把业务表结构映射为框架层可理解的UI打开参数
    /// </summary>
    private static UIFormOpenOptions CreateOptions(Sys_UIFormEntity entity)
    {
        return new UIFormOpenOptions
        {
            UIGroupId = entity.UIGroupId,
            AllowMultiInstance = entity.CanMulit == 1,
            DisableLayer = entity.DisableUILayer == 1,
            IsResident = entity.IsLock == 1,
            ShowMode = (EUIFormShowMode)entity.ShowMode,
        };
    }

}
