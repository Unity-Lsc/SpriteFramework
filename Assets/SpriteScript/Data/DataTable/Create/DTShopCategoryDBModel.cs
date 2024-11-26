
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTShopCategory数据管理
    /// </summary>
    public partial class DTShopCategoryDBModel : DataTableDBModelBase<DTShopCategoryDBModel, DTShopCategoryEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTShopCategory"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTShopCategoryEntity entity = new DTShopCategoryEntity();
                entity.Id = ms.ReadInt();
                entity.Name = ms.ReadUTF8String();

                _entityList.Add(entity);
                _entityDict[entity.Id] = entity;
            }
        }
    }
}