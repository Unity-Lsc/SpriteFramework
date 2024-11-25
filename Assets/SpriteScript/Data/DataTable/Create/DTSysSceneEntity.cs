using System.Collections;

namespace SpriteFramework
{
    /// <summary>
      /// DTSysScene实体
    /// </summary>
    public partial class DTSysSceneEntity : DataTableEntityBase
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc;

        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// 名称
        /// </summary>
        public string SceneName;

        /// <summary>
        /// 背景音乐
        /// </summary>
        public int BGMId;

        /// <summary>
        /// 场景类型(0=登录1=选人2=PVP)
        /// </summary>
        public int SceneType;

        /// <summary>
        /// 玩家出生点x
        /// </summary>
        public float PlayerBornPos_1;

        /// <summary>
        /// 玩家出生点y
        /// </summary>
        public float PlayerBornPos_2;

        /// <summary>
        /// 玩家出生点z
        /// </summary>
        public float PlayerBornPos_3;

    }
}
