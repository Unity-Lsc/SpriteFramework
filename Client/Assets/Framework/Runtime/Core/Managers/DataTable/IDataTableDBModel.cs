using Cysharp.Threading.Tasks;

namespace SpriteFramework
{
       /// <summary>
    /// 数据表管理接口
    /// </summary>
    public interface IDataTableDBModel
    {
        /// <summary>
        /// 数据表名称
        /// </summary>
        string DataTableName { get; }

        /// <summary>
        /// 加载数据表数据
        /// </summary>
        UniTask LoadData();

        /// <summary>
        /// 清空数据
        /// </summary>
        void Clear();
    }
}
