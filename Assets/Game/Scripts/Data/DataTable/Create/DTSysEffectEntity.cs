using System.Collections;

namespace SpriteFramework
{
    /// <summary>
      /// DTSysEffect实体
    /// </summary>
    public partial class DTSysEffectEntity : DataTableEntityBase
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc;

        /// <summary>
        /// 预设编号
        /// </summary>
        public int PrefabId;

        /// <summary>
        /// 持续时间(秒)
        /// </summary>
        public float KeepTime;

        /// <summary>
        /// 声音编号
        /// </summary>
        public int SoundId;

        /// <summary>
        /// 特效类型(0=普通1=抛物线)
        /// </summary>
        public int Type;

    }
}
