
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTSysCommonEventId数据管理
    /// </summary>
    public partial class DTSysCommonEventIdDBModel : DataTableDBModelBase<DTSysCommonEventIdDBModel, DTSysCommonEventIdEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTSysCommonEventId"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTSysCommonEventIdEntity entity = new DTSysCommonEventIdEntity();
                entity.Id = ms.ReadInt();
                entity.Desc = ms.ReadUTF8String();
                entity.Name = ms.ReadUTF8String();

                _entityList.Add(entity);
                _entityDict[entity.Id] = entity;
            }
        }
    }
}