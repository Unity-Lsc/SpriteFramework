namespace SpriteFramework
{
    /// <summary>
    /// 安全的Int类型（进行移位加密操作）
    /// </summary>
    public struct SafeInteger
    {

        private int m_EncryptValue;

        private const int m_Mask = 9981;

        /// <summary>
        /// 真实值（方便在Lua中调用）
        /// </summary>
        public int RealValue {
            get {
                int v = m_EncryptValue ^ m_Mask;
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
            safeInteger.m_EncryptValue = n ^ m_Mask;
            return safeInteger;
        }  

        public static implicit operator int(SafeInteger safeInteger) {
            int v = safeInteger ^ m_Mask;
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
