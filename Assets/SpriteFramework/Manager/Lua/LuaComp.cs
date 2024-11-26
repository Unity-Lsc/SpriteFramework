using UnityEngine;

namespace SpriteFramework
{

    /// <summary>
    /// Lua组件类型
    /// </summary>
    public enum LuaCompType
    {
        GameObject = 0,
        Transform,
        Button,
        Image,
        Text,
        RawImage,
        InputField,
        Scrollbar,
        ScrollRect,
        UIMultiScroller,
        UIScroller,
    }

    /// <summary>
    /// Lua组件
    /// </summary>
    public class LuaComp
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 类型
        /// </summary>
        public LuaCompType Type;

        /// <summary>
        /// Transform
        /// </summary>
        public Transform Tran;

    }
}
