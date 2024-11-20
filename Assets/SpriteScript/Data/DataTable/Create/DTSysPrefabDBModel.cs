
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTSysPrefab数据管理
    /// </summary>
    public partial class DTSysPrefabDBModel : DataTableDBModelBase<DTSysPrefabDBModel, DTSysPrefabEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTSysPrefab"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTSysPrefabEntity entity = new DTSysPrefabEntity();
                entity.Id = ms.ReadInt();
                entity.Desc = ms.ReadUTF8String();
                entity.Name = ms.ReadUTF8String();
                entity.AssetCategory = ms.ReadInt();
                entity.AssetPath = ms.ReadUTF8String();
                entity.PoolId = (byte)ms.ReadByte();
                entity.CullDespawned = (byte)ms.ReadByte();
                entity.CullAbove = ms.ReadInt();
                entity.CullDelay = ms.ReadInt();
                entity.CullMaxPerPass = ms.ReadInt();

                m_List.Add(entity);
                m_Dic[entity.Id] = entity;
            }
        }
    }
}