using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 协议编号
/// </summary>
public class ProtoIdDefine
{

    public const ushort NetRecvCmdEvent = 1001;
    public const ushort NetDisconnectEvent = 1002;
    public const ushort NetConnectedEvent = 1003;
    public const ushort NetConnectingEvent = 1004;

    /// <summary>
    /// 玩家向网关服务器注册客户端
    /// </summary>
    public const ushort Proto_C2GWS_RegClient = 10001;

    /// <summary>
    /// 玩家向网关服务器发送进入场景申请消息
    /// </summary>
    public const ushort Proto_C2GWS_EnterScene_Apply = 10002;

    /// <summary>
    /// 玩家向网关服务器发送进入场景消息
    /// </summary>
    public const ushort Proto_C2GWS_EnterScene = 10003;

    /// <summary>
    /// 玩家向网关服务器发送心跳消息
    /// </summary>
    public const ushort Proto_C2GWS_Heartbeat = 10004;

    /// <summary>
    /// 服务器返回进入场景申请
    /// </summary>
    public const ushort Proto_GS2C_ReturnEnterScene_Apply = 15001;

}
