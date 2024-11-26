
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTSysScene数据管理
    /// </summary>
    public partial class DTSysSceneDBModel : DataTableDBModelBase<DTSysSceneDBModel, DTSysSceneEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTSysScene"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTSysSceneEntity entity = new DTSysSceneEntity();
                entity.Id = ms.ReadInt();
                entity.Desc = ms.ReadUTF8String();
                entity.Name = ms.ReadUTF8String();
                entity.SceneName = ms.ReadUTF8String();
                entity.BGMId = ms.ReadInt();
                entity.SceneType = ms.ReadInt();
                entity.PlayerBornPos_1 = ms.ReadFloat();
                entity.PlayerBornPos_2 = ms.ReadFloat();
                entity.PlayerBornPos_3 = ms.ReadFloat();

                _entityList.Add(entity);
                _entityDict[entity.Id] = entity;
            }
        }
    }
}