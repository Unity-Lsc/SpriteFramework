using UnityEngine;
using XLua;

namespace SpriteFramework
{
    /// <summary>
    /// Lua管理器
    /// </summary>
    public class LuaManager
    {

        private readonly LuaEnv _luaEnv;

        public LuaManager() {
            //1.实例化xLua引擎
            _luaEnv = new LuaEnv();
            //2.设置xLua的脚本路径
            _luaEnv.DoString(string.Format("package.path = '{0}/?.bytes'", Application.dataPath + "/Game/AssetsPackage/xLuaLogic/"));

            DoString("Main");
        }

        public void DoString(string str) {
            _luaEnv.DoString(string.Format("require '{0}'", str));
        }

    }
}
