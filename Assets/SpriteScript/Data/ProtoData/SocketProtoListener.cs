using SpriteFramework;

public sealed class SocketProtoListener
{
    /// <summary>
    /// 添加协议监听
    /// </summary>
    public static void AddProtoListener() {
        //GameEntry.Event.AddListener(ProtoIdDefine.NetRecvCmdEvent, (object param)=> {
        //    GameEntry.Log("ProtoIdDefine.NetRecvCmdEvent  参数:{0}", LogCategory.Proto, param);
        //});
        //GameEntry.Event.AddListener(ProtoIdDefine.NetDisconnectEvent, (object param) => {
        //    GameEntry.Log("ProtoIdDefine.NetDisconnectEvent  参数:{0}", LogCategory.Proto, param);
        //});
        //GameEntry.Event.AddListener(ProtoIdDefine.NetConnectedEvent, (object param) => {
        //    GameEntry.Log("ProtoIdDefine.NetConnectedEvent  参数:{0}", LogCategory.Proto, param);
        //});
        //GameEntry.Event.AddListener(ProtoIdDefine.NetConnectingEvent, (object param) => {
        //    GameEntry.Log("ProtoIdDefine.NetConnectingEvent  参数:{0}", LogCategory.Proto, param);
        //});
    }

    /// <summary>
    /// 移除协议监听
    /// </summary>
    public static void RemoveProtoListener() {
        //GameEntry.Event.RemoveListener(ProtoIdDefine.NetRecvCmdEvent);
        //GameEntry.Event.RemoveListener(ProtoIdDefine.NetDisconnectEvent);
        //GameEntry.Event.RemoveListener(ProtoIdDefine.NetConnectedEvent);
        //GameEntry.Event.RemoveListener(ProtoIdDefine.NetConnectingEvent);
    }

}
