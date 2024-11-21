using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// Transform变量
    /// </summary>
    public class VarTransform : Variable<Transform>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        public static VarTransform Alloc(Transform value = null)
        {
            VarTransform var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarTransform>();
            var.Value = value;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// VarTransform -> Transform
        /// </summary>
        public static implicit operator Transform(VarTransform value)
        {
            return value.Value;
        }
    }
}