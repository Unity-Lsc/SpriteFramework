using System.IO;
using YooAsset.Editor;

/// <summary>
/// 收集.bytes文件的过滤规则
/// </summary>
public class CollectBytes : IFilterRule
{
    public string FindAssetType => "";

    public bool IsCollectAsset(FilterRuleData data)
    {
        return Path.GetExtension(data.AssetPath).Equals(".bytes", System.StringComparison.OrdinalIgnoreCase);
    }
}
