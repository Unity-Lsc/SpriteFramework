namespace SpriteFramework
{
    /// <summary>
    /// 协议接口
    /// </summary>
    public interface IProto
    {

        /// <summary>
        /// 协议编号
        /// </summary>
        ushort ProtoCode { get; }

        string ProtoEnName { get; }

        byte[] ToArray();

    }
}
