
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTTask数据管理
    /// </summary>
    public partial class DTTaskDBModel : DataTableDBModelBase<DTTaskDBModel, DTTaskEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTTask"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTTaskEntity entity = new DTTaskEntity();
                entity.Id = ms.ReadInt();
                entity.Name = ms.ReadUTF8String();
                entity.Status = ms.ReadInt();
                entity.Content = ms.ReadUTF8String();

                _entityList.Add(entity);
                _entityDict[entity.Id] = entity;
            }
        }
    }
}