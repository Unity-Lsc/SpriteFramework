using UnityEngine;
using XLua;
using SpriteMain;

namespace SpriteFramework
{
    /// <summary>
    /// Lua管理器
    /// </summary>
    public class LuaManager
    {
        /// <summary>
        /// 全局的xLua引擎
        /// </summary>
        public static LuaEnv LuaEnv;

        public LuaManager() {
            //实例化xLua引擎
            LuaEnv = new LuaEnv();

            if(MainEntry.Instance.PlayMode == YooAsset.EPlayMode.EditorSimulateMode) {
                //设置xLua的脚本路径
                LuaEnv.DoString(string.Format("package.path = '{0}/?.bytes'", Application.dataPath + "/AssetsPackage/xLuaLogic/"));
            } else {
                //添加自定义Loader
                LuaEnv.AddLoader(MyLoader);
            }
            DoString("Main");
        }

        /// <summary>
        /// 自定义的Loader
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private byte[] MyLoader(ref string filePath) {
            TextAsset asset = GameEntry.Resource.LoadLua(filePath);
            byte[] buffer = asset.bytes;
            if (buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191) {
                // 处理UTF-8 BOM头
                buffer[0] = buffer[1] = buffer[2] = 32;
            }
            return buffer;
        }

        public void DoString(string str) {
            LuaEnv.DoString(string.Format("require '{0}'", str));
        }

    }
}
