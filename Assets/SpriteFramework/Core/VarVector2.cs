using UnityEngine;

namespace SpriteFramework
{
    public class VarVector2 : Variable<Vector2>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        public static VarVector2 Alloc()
        {
            VarVector2 var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarVector2>();
            var.Value = Vector2.zero; ;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        public static VarVector2 Alloc(VarVector2 value)
        {
            VarVector2 var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// VarString -> string
        /// </summary>
        public static implicit operator Vector2(VarVector2 value)
        {
            return value.Value;
        }
    }
}