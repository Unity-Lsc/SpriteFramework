namespace SpriteFramework
{
    /// <summary>
    /// 安全的Int类型（进行移位加密操作）
    /// </summary>
    public struct SafeInteger
    {

        private int _encryptValue;

        private const int _mask = 9981;

        /// <summary>
        /// 真实值（方便在Lua中调用）
        /// </summary>
        public int RealValue {
            get {
                int v = _encryptValue ^ _mask;
                return (int)((uint)v << 16 | (uint)v >> 16);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="n">初始值</param>
        public static implicit operator SafeInteger(int n) {
            SafeInteger safeInteger;
            n = (int)((uint)n << 16 | (uint)n >> 16);
            safeInteger._encryptValue = n ^ _mask;
            return safeInteger;
        }  

        public static implicit operator int(SafeInteger safeInteger) {
            int v = safeInteger ^ _mask;
            return (int)((uint)v << 16 | (uint)v >> 16);
        }

        public static SafeInteger operator ++(SafeInteger safeInteger) {
            safeInteger += 1;
            return safeInteger;
        }

        public static SafeInteger operator --(SafeInteger safeInteger) {
            safeInteger -= 1;
            return safeInteger;
        }

        public override string ToString() {
            int v = (int)this;
            return v.ToString();
        }

    }
}
