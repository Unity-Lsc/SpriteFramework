using System.IO;
using SpriteMain;

namespace SpriteFramework
{
    public class SFConstDefine
    {
        /// <summary>
        /// 资源依赖信息文件名称
        /// </summary>
        public const string AssetInfoName = "AssetInfo.bytes";

        /// <summary>
        /// 框架编辑器层的文件夹路径
        /// </summary>
        public const string FrameworkEditorFolderPath = "Assets/Framework/Editor/";

        /// <summary>
        /// 框架启动层的文件夹路径
        /// </summary>
        public const string FrameworkLaunchFolderPath = "Assets/Framework/Runtime/Launch/";

        /// <summary>
        /// 框架核心代码层的文件夹路径
        /// </summary>
        public const string FrameworkCoreFolderPath = "Assets/Framework/Runtime/Core/";

        /// <summary>
        /// 框架功能系统层的文件夹路径
        /// </summary>
        public const string FramewrokSystemFolderPath = "Assets/Framework/Runtime/System/";

        /// <summary>
        /// HybridCLR数据的路径
        /// </summary>
        public const string HybridCLRDataPath = "Assets/HybridCLRData/";

        /// <summary>
        /// 数据表的AssetBundle的存储路径
        /// </summary>
        public const string DataTableAssetBundlePath = "game/download/datatable.assetbundle";

        /// <summary>
        /// 自定义Shader的AssetBundle的存储路径
        /// </summary>
        public const string CusShadersAssetBundlePath = "game/download/shader.assetbundle";

        /// <summary>
        /// 可写区资源依赖信息文件路径
        /// </summary>
        public static string LocalAssetInfoPath = Path.Combine(MainConstDefine.LocalAssetBundlePath, AssetInfoName);
    }
}