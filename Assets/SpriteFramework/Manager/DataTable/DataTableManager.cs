using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 数据表管理器
    /// </summary>
    public class DataTableManager
    {

        public DTRechargeShopDBModel DTRechargeShopDBModel { get; private set; }

        /// <summary>
        /// 加载数据表的数据
        /// </summary>
        internal void LoadDataTable() {
            DTRechargeShopDBModel = new DTRechargeShopDBModel();
            DTRechargeShopDBModel.LoadData();
        }

        /// <summary>
        /// 获取表格的字节数组
        /// </summary>
        internal byte[] GetDataTableBuffer(string dataTableName) {
            TextAsset asset = GameEntry.Resource.LoadDataTable(dataTableName);
            if (asset == null) return null;
            return asset.bytes;
        }

        public void Dispose() {
            DTRechargeShopDBModel.Clear();
        }

    }
}
