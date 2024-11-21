namespace SpriteFramework
{
    /// <summary>
    /// byte变量
    /// </summary>
    public class VarByte : Variable<byte>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        public static VarByte Alloc(byte value = 0) {
            VarByte var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarByte>();
            var.Value = value;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// VarByte -> byte
        /// </summary>
        public static implicit operator byte(VarByte value) {
            return value.Value;
        }

    }
}
