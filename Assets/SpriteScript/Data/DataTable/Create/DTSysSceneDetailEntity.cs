using System.Collections;

namespace SpriteFramework
{
    /// <summary>
      /// DTSysSceneDetail实体
    /// </summary>
    public partial class DTSysSceneDetailEntity : DataTableEntityBase
    {
        /// <summary>
        /// 场景编号
        /// </summary>
        public int SceneId;

        /// <summary>
        /// 场景名字
        /// </summary>
        public string SceneName;

        /// <summary>
        /// 场景路径
        /// </summary>
        public string ScenePath;

        /// <summary>
        /// 场景等级(0=必须1=重要2=不重要)
        /// </summary>
        public int SceneGrade;

    }
}
