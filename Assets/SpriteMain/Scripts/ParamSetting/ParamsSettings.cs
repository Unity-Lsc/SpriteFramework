using UnityEngine;

[CreateAssetMenu(menuName = "YooAsset/ParamsSettings", fileName = "ParamsSettings")]
public class ParamsSettings : ScriptableObject
{
    [Header("Http请求相关")]
    #region Http请求相关

    [Tooltip("正式服请求路径")]
    /// <summary>
    /// 正式服请求路径
    /// </summary>
    public string WebAccountUrl;

    [Tooltip("测试服请求路径")]
    /// <summary>
    /// 测试服请求路径
    /// </summary>
    public string TestWebAccountUrl;

    /// <summary>
    /// Http请求的重试次数
    /// </summary>
    [Tooltip("Http请求的重试次数")]
    public int HttpRetry = 3;

    /// <summary>
    /// Http请求的重试间隔
    /// </summary>
    [Tooltip("Http请求的重试间隔")]
    public int HttpRetryInterval = 3;

    [Tooltip("当前是否测试服")]
    /// <summary>
    /// 当前是否测试服
    /// </summary>
    public bool IsTest;

    [Tooltip("是否加密")]
    /// <summary>
    /// 是否加密
    /// </summary>
    public bool PostIsEncrypt;

    [Tooltip("设置Post的ContentType")]
    /// <summary>
    /// 设置Post的ContentType
    /// </summary>
    public string PostContentType;

    #endregion Http请求相关end

    [Space(10)]

    [Header("系统参数")]
    #region 系统参数

    /// <summary>
    /// 类对象池_释放间隔
    /// </summary>
    [Tooltip("类对象池_释放间隔")]
    public int PoolReleaseClassObjectInterval = 30;

    /// <summary>
    /// UI界面池_回池后过期时间_秒
    /// </summary>
    [Tooltip("UI界面池_回池后过期时间_秒")]
    public int UIExpire = 30;

    /// <summary>
    /// UI界面池_释放间隔_秒
    /// </summary>
    [Tooltip("UI界面池_释放间隔_秒")]
    public int UIClearInterval = 30;

    #endregion 系统参数end

    [Space(10)]

    [Header("业务系统测试")]
    #region 业务系统测试

    /// <summary>
    /// 是否测试手机端Input系统
    /// </summary>
    [Tooltip("是否测试手机端Input系统")]
    public bool MobileDebug;

    /// <summary>
    /// 是否激活新手引导
    /// </summary>
    [Tooltip("是否激活新手引导")]
    public bool ActiveGuide;

    #endregion

}
