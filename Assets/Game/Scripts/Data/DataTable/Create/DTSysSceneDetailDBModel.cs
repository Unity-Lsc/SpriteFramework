
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTSysSceneDetail数据管理
    /// </summary>
    public partial class DTSysSceneDetailDBModel : DataTableDBModelBase<DTSysSceneDetailDBModel, DTSysSceneDetailEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTSysSceneDetail"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTSysSceneDetailEntity entity = new DTSysSceneDetailEntity();
                entity.Id = ms.ReadInt();
                entity.SceneId = ms.ReadInt();
                entity.SceneName = ms.ReadUTF8String();
                entity.ScenePath = ms.ReadUTF8String();
                entity.SceneGrade = ms.ReadInt();

                m_List.Add(entity);
                m_Dic[entity.Id] = entity;
            }
        }
    }
}