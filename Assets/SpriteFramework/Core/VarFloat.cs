namespace SpriteFramework
{
    /// <summary>
    /// float变量
    /// </summary>
    public class VarFloat : Variable<float>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        public static VarFloat Alloc(float value = 0)
        {
            VarFloat var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarFloat>();
            var.Value = value;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// VarFloat -> float
        /// </summary>
        public static implicit operator float(VarFloat value)
        {
            return value.Value;
        }
    }
}