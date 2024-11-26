
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTSysEffect数据管理
    /// </summary>
    public partial class DTSysEffectDBModel : DataTableDBModelBase<DTSysEffectDBModel, DTSysEffectEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTSysEffect"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTSysEffectEntity entity = new DTSysEffectEntity();
                entity.Id = ms.ReadInt();
                entity.Desc = ms.ReadUTF8String();
                entity.PrefabId = ms.ReadInt();
                entity.KeepTime = ms.ReadFloat();
                entity.SoundId = ms.ReadInt();
                entity.Type = ms.ReadInt();

                _entityList.Add(entity);
                _entityDict[entity.Id] = entity;
            }
        }
    }
}