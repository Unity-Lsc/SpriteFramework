
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTMaterials数据管理
    /// </summary>
    public partial class DTMaterialsDBModel : DataTableDBModelBase<DTMaterialsDBModel, DTMaterialsEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTMaterials"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTMaterialsEntity entity = new DTMaterialsEntity();
                entity.Id = ms.ReadInt();
                entity.Name = ms.ReadUTF8String();
                entity.Quality = ms.ReadInt();
                entity.Description = ms.ReadUTF8String();
                entity.Type = ms.ReadInt();
                entity.FixedType = ms.ReadInt();
                entity.FixedAddValue = ms.ReadInt();
                entity.maxAmount = ms.ReadInt();
                entity.packSort = ms.ReadInt();
                entity.CompositionProps = ms.ReadUTF8String();
                entity.CompositionMaterialID = ms.ReadInt();
                entity.CompositionGold = ms.ReadUTF8String();
                entity.SellMoney = ms.ReadInt();

                m_List.Add(entity);
                m_Dic[entity.Id] = entity;
            }
        }
    }
}