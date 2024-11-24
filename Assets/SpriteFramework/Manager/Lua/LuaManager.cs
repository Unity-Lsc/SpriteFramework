using UnityEngine;
using XLua;

namespace SpriteFramework
{
    /// <summary>
    /// Lua管理器
    /// </summary>
    public class LuaManager
    {

        private readonly LuaEnv m_LuaEnv;

        public LuaManager() {
            //1.实例化xLua引擎
            m_LuaEnv = new LuaEnv();
            //2.设置xLua的脚本路径
            m_LuaEnv.DoString(string.Format("package.path = '{0}/?.bytes'", Application.dataPath + "/Game/AssetsPackage/xLuaLogic/"));

            DoString("Main");
        }

        public void DoString(string str) {
            m_LuaEnv.DoString(string.Format("require '{0}'", str));
        }

    }
}
