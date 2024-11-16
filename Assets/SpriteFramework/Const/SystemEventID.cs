namespace SpriteFramework
{
    /// <summary>
    /// 系统事件编号(采用4位数字 1001(10表示模块,01表示编号))
    /// </summary>
    public class SystemEventID
    {

        /// <summary>
        /// 加载表格完毕
        /// </summary>
        public const ushort LoadDataTableComplete = 1001;

        /// <summary>
        /// 加载单一表格完毕
        /// </summary>
        public const ushort LoadOneDataTableComplete = 1002;

        /// <summary>
        /// 加载Lua表格完毕
        /// </summary>
        public const ushort LoadLuaDataTableComplete = 1003;

        /// <summary>
        /// 加载进度条更新
        /// </summary>
        public const ushort LoadingProgressChange = 1004;

        /// <summary>
        /// 检查版本更新开始下载
        /// </summary>
        public const ushort CheckVersionBeginDownload = 1201;

        /// <summary>
        /// 检查版本更新下载资源中
        /// </summary>
        public const ushort CheckVersionDownloadUpdate = 1202;

        /// <summary>
        /// 检查版本更新下载完毕
        /// </summary>
        public const ushort CheckVersionDownloadComplete = 1203;

    }
}
