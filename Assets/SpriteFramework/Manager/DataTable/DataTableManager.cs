using System.Collections.Generic;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// 数据表管理器
    /// </summary>
    public class DataTableManager
    {

        public LocalizationDBModel LocalizationDBModel { get; private set; }

        public DTEquipDBModel DTEquipDBModel { get; private set; }
        public DTItemDBModel DTItemDBModel { get; private set; }
        public DTMaterialsDBModel DTMaterialsDBModel { get; private set; }
        public DTRechargeShopDBModel DTRechargeShopDBModel { get; private set; }
        public DTShopCategoryDBModel DTShopCategoryDBModel { get; private set; }
        public DTShopDBModel DTShopDBModel { get; private set; }
        public DTSysAudioDBModel DTSysAudioDBModel { get; private set; }
        public DTSysCodeDBModel DTSysCodeDBModel { get; private set; }
        public DTSysCommonEventIdDBModel DTSysCommonEventIdDBModel { get; private set; }
        public DTSysConfigDBModel DTSysConfigDBModel { get; private set; }
        public DTSysEffectDBModel DTSysEffectDBModel { get; private set; }
        public DTSysPrefabDBModel DTSysPrefabDBModel { get; private set; }
        public DTSysSceneDBModel DTSysSceneDBModel { get; private set; }
        public DTSysSceneDetailDBModel DTSysSceneDetailDBModel { get; private set; }
        public DTSysStorySoundDBModel DTSysStorySoundDBModel { get; private set; }
        public DTSysUIFormDBModel DTSysUIFormDBModel { get; private set; }
        public DTTaskDBModel DTTaskDBModel { get; private set; }


        /// <summary>
        /// 加载数据表的数据
        /// </summary>
        internal void LoadDataTable() {
            //LocalizationDBModel不需要在这里LoadData, 而是在LocalizationManager里面LoadData
            LocalizationDBModel = new LocalizationDBModel();

            DTEquipDBModel = new DTEquipDBModel();DTEquipDBModel.LoadData();
            DTItemDBModel = new DTItemDBModel();DTItemDBModel.LoadData();
            DTMaterialsDBModel = new DTMaterialsDBModel();DTMaterialsDBModel.LoadData();
            DTRechargeShopDBModel = new DTRechargeShopDBModel();DTRechargeShopDBModel.LoadData();
            DTShopCategoryDBModel = new DTShopCategoryDBModel(); DTShopCategoryDBModel.LoadData();
            DTShopDBModel = new DTShopDBModel(); DTShopDBModel.LoadData();
            DTSysAudioDBModel = new DTSysAudioDBModel(); DTSysAudioDBModel.LoadData();
            DTSysCodeDBModel = new DTSysCodeDBModel(); DTSysCodeDBModel.LoadData();
            DTSysCommonEventIdDBModel = new DTSysCommonEventIdDBModel(); DTSysCommonEventIdDBModel.LoadData();
            DTSysConfigDBModel = new DTSysConfigDBModel(); DTSysConfigDBModel.LoadData();
            DTSysEffectDBModel = new DTSysEffectDBModel(); DTSysEffectDBModel.LoadData();
            DTSysPrefabDBModel = new DTSysPrefabDBModel(); DTSysPrefabDBModel.LoadData();
            DTSysSceneDBModel = new DTSysSceneDBModel(); DTSysSceneDBModel.LoadData();
            DTSysSceneDetailDBModel = new DTSysSceneDetailDBModel(); DTSysSceneDetailDBModel.LoadData();
            DTSysStorySoundDBModel = new DTSysStorySoundDBModel(); DTSysStorySoundDBModel.LoadData();
            DTSysUIFormDBModel = new DTSysUIFormDBModel(); DTSysUIFormDBModel.LoadData();
            DTTaskDBModel = new DTTaskDBModel(); DTTaskDBModel.LoadData();
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
