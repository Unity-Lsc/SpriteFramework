
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTShop数据管理
    /// </summary>
    public partial class DTShopDBModel : DataTableDBModelBase<DTShopDBModel, DTShopEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTShop"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTShopEntity entity = new DTShopEntity();
                entity.Id = ms.ReadInt();
                entity.ShopCategoryId = ms.ReadInt();
                entity.GoodsType = ms.ReadInt();
                entity.GoodsId = ms.ReadInt();
                entity.OldPrice = ms.ReadInt();
                entity.Price = ms.ReadInt();
                entity.SellStatus = ms.ReadInt();

                _entityList.Add(entity);
                _entityDict[entity.Id] = entity;
            }
        }
    }
}