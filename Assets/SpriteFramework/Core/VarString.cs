namespace SpriteFramework
{
    /// <summary>
    /// string变量
    /// </summary>
    public class VarString : Variable<string>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        public static VarString Alloc()
        {
            VarString var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarString>();
            var.Value = string.Empty;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        public static VarString Alloc(string value)
        {
            VarString var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// VarString -> string
        /// </summary>
        public static implicit operator string(VarString value)
        {
            return value.Value;
        }
    }
}