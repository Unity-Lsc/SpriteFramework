namespace SpriteFramework
{
    /// <summary>
    /// byte[]变量
    /// </summary>
    public class VarBytes : Variable<byte[]>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        public static VarBytes Alloc(byte[] value= null) {
            VarBytes var = GameEntry.Pool.VarObjectPool.DequeueVarObject<VarBytes>();
            var.Value = value;
            var.AddRefCount();
            return var;
        }

        /// <summary>
        /// VarBytes -> byte[]
        /// </summary>
        public static implicit operator byte[](VarBytes value) {
            return value.Value;
        }
    }
}
