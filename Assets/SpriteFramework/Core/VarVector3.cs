using UnityEngine;


namespace SpriteFramework
{
    public class VarVector3 : Variable<Vector3>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        public static VarVector3 Alloc()
        {
            VarVector3 var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarVector3>();
            var.Value = Vector3.zero; ;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        public static VarVector3 Alloc(VarVector3 value)
        {
            VarVector3 var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// VarString -> string
        /// </summary>
        public static implicit operator Vector3(VarVector3 value)
        {
            return value.Value;
        }
    }
}