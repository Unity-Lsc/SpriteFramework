namespace SpriteFramework
{
    /// <summary>
    /// int变量
    /// </summary>
    public class VarInt : Variable<int>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        public static VarInt Alloc(int value = 0)
        {
            VarInt var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarInt>();
            var.Value = value;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// VarInt -> int
        /// </summary>
        public static implicit operator int(VarInt value)
        {
            return value.Value;
        }
    }
}