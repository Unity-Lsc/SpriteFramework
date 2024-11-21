using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// GameObject变量
    /// </summary>
    public class VarGameObject : Variable<GameObject>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        public static VarGameObject Alloc(GameObject value = null)
        {
            VarGameObject var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarGameObject>();
            var.Value = value;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// VarGameObject -> GameObject
        /// </summary>
        public static implicit operator GameObject(VarGameObject value)
        {
            return value.Value;
        }
    }
}