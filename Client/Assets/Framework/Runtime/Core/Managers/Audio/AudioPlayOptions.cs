namespace SpriteFramework
{
    /// <summary>
    /// 音效播放参数(业务侧可以自由决定这些参数来自配置表、代码常量还是ScriptableObject等其他配置源)
    /// </summary>
    public sealed class AudioPlayOptions
    {
        /// <summary>
        /// 音量
        /// </summary>
        public float Volume = 1f;

        /// <summary>
        /// AudioSource 优先级
        /// </summary>
        public int Priority = 128;
    }
}
