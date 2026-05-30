namespace SpriteFramework
{
    /// <summary>
    /// BGM播放参数(业务侧可以自由决定这些参数来自配置表、代码常量还是ScriptableObject等其他配置源)
    /// </summary>
    public class BGMPlayOptions
    {
        /// <summary>
        /// BGM 音量
        /// </summary>
        public float Volume = 1f;

        /// <summary>
        /// 预留优先级字段，便于未来扩展
        /// 当前 BGMSource 是单实例，该字段主要保持和业务侧配置对齐
        /// </summary>
        public int Priority = 128;

        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool IsLoop;

        /// <summary>
        /// 是否淡入
        /// </summary>
        public bool IsFadeIn;

        /// <summary>
        /// 是否淡出
        /// </summary>
        public bool IsFadeOut;
    }
}
