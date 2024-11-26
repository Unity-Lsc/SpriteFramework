
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpriteFramework
{
    /// <summary>
    /// DTSysAudio数据管理
    /// </summary>
    public partial class DTSysAudioDBModel : DataTableDBModelBase<DTSysAudioDBModel, DTSysAudioEntity>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName { get { return "DTSysAudio"; } }

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(SpriteMemoryStream ms)
        {
            int rows = ms.ReadInt();
            int columns = ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                DTSysAudioEntity entity = new DTSysAudioEntity();
                entity.Id = ms.ReadInt();
                entity.Desc = ms.ReadUTF8String();
                entity.AssetPath = ms.ReadUTF8String();
                entity.Is3D = ms.ReadInt();
                entity.Volume = ms.ReadFloat();

                _entityList.Add(entity);
                _entityDict[entity.Id] = entity;
            }
        }
    }
}