namespace SpriteFramework
{
    /// <summary>
    /// long变量
    /// </summary>
    public class VarLong : Variable<long>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarLong Alloc(long value = 0)
        {
            VarLong var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarLong>();
            var.Value = value;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// VarLong -> long
        /// </summary>
        public static implicit operator long(VarLong value)
        {
            return value.Value;
        }
    }
}