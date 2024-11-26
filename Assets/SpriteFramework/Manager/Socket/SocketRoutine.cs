using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace SpriteFramework
{
    /// <summary>
    /// Socket Tcp 访问器
    /// </summary>
    public class SocketTcpRoutine
    {
        //发送消息队列
        private Queue<byte[]> _sendQueue = new Queue<byte[]>();
        //压缩数组的长度界限
        private const int _compressLen = 200;

        //是否连接成功
        private bool _isConnectedSuccess;

        //接收数据包的字节数组缓冲区
        private byte[] _receiveBuffer = new byte[1024];
        //接收数据包的缓冲数据流
        private SpriteMemoryStream _receiveMS = new SpriteMemoryStream();

        /// <summary>
        /// 发送用的MemoryStream
        /// </summary>
        private SpriteMemoryStream _socketSendMS = new SpriteMemoryStream();
        /// <summary>
        /// 接收用的MemoryStream
        /// </summary>
        private SpriteMemoryStream _socketReceiveMS = new SpriteMemoryStream();

        //接收消息的队列
        private Queue<byte[]> _receiveQueue = new Queue<byte[]>();
        //接收消息的数量
        private int _receiveCount = 0;

        //这一帧发送了多少
        private int _sendCount = 0;

        //是否有未处理的字节
        private bool _isHasUnDealBytes = false;

        //未处理的字节
        private byte[] _unDealBytes = null;

        /// <summary>
        /// 客户端socket
        /// </summary>
        private Socket _client;

        /// <summary>
        /// 连接成功的回调
        /// </summary>
        public Action OnConnectSuccess;


        internal void OnUpdate() {
            if (_isConnectedSuccess) {
                _isConnectedSuccess = false;
                OnConnectSuccess?.Invoke();
                Debug.Log("Socket Tcp 连接成功");
            }

            //从队列中获取消息
            while (true) {
                if (_receiveCount <= GameEntry.Socket.MaxReceiveCount) {
                    _receiveCount++;
                    lock (_receiveQueue) {
                        if (_receiveQueue.Count > 0) {
                            //得到队列中的数据包
                            byte[] buffer = _receiveQueue.Dequeue();
                            //异或之后的数组
                            byte[] bufferNew = new byte[buffer.Length - 3];

                            bool isCompress = false;
                            ushort crc = 0;

                            using (SpriteMemoryStream ms = new SpriteMemoryStream(buffer)) {
                                isCompress = ms.ReadBool();
                                crc = ms.ReadUShort();
                                ms.Read(bufferNew, 0, bufferNew.Length);
                            }

                            //先CRC
                            int newCRC = Crc16.CalculateCrc16(bufferNew);

                            if (crc == newCRC) {
                                //异或 得到原始数据
                                bufferNew = SecurityUtil.Xor(bufferNew);

                                if (isCompress) {
                                    bufferNew = ZlibHelper.DeCompressBytes(bufferNew);
                                }

                                ushort protoCode = 0;
                                byte[] protoContent = new byte[bufferNew.Length - 2];
                                using (SpriteMemoryStream ms = new SpriteMemoryStream(bufferNew)) {
                                    //协议编号
                                    protoCode = ms.ReadUShort();
                                    ms.Read(protoContent, 0, protoContent.Length);

                                    GameEntry.Event.Dispatch(protoCode, protoContent);
                                }

                            } else {
                                break;
                            }

                        } else {
                            break;
                        }
                    }
                } else {
                    _receiveCount = 0;
                    break;
                }
            }

            CheckSendQueue();

        }

        /// <summary>
        /// 连接到Socket服务器
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">端口号</param>
        public void Connect(string ip, int port) {
            //如果Socket已经存在,并且处于连接中状态 则直接返回
            if (_client != null && _client.Connected) return;

            string newServerIP = ip;
            AddressFamily addressFamily = AddressFamily.InterNetwork;

#if UNITY_IPHONE && !UNITY_EDITOR && SDKCHANNEL_APPLE_STORE
            AppleStoreInterface.GetIPv6Type(ip, port.ToString(), out newServerIp, out addressFamily);
#endif
            _client = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                _client.BeginConnect(new IPEndPoint(IPAddress.Parse(newServerIP), port), ConnectCallBack, _client);
            } catch (Exception ex) {
                Debug.Log("连接失败:" + ex.Message);
            }

        }

        private void ConnectCallBack(IAsyncResult ar) {
            if (_client.Connected) {
                Debug.Log("Socket连接成功");
                GameEntry.Socket.RegisterSocketTcpRoutine(this);

                ReceiveMsg();
                _isConnectedSuccess = true;
            } else {
                Debug.Log("连接失败");
            }
            _client.EndConnect(ar);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect() {
            if (_client != null && _client.Connected) {
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
                GameEntry.Socket.RemoveSocketTcpRoutine(this);
            }
        }

        /// <summary>
        /// 检查发送队列
        /// </summary>
        private void CheckSendQueue() {
            if (_sendCount >= GameEntry.Socket.MaxSendCount) {
                //等待下一帧发送
                _sendCount = 0;
                return;
            }
            lock (_sendQueue) {
                if (_sendQueue.Count > 0 || _isHasUnDealBytes) {
                    int smallCount = 0;//拼凑的小包数量
                    SpriteMemoryStream ms = _socketSendMS;
                    ms.SetLength(0);

                    //先处理 未处理的包
                    if (_isHasUnDealBytes) {
                        _isHasUnDealBytes = false;
                        ms.Write(_unDealBytes, 0, _unDealBytes.Length);
                        smallCount++;
                    }
                    //再检查队列中的包
                    while (true) {
                        if (_sendQueue.Count == 0) break;
                        //取出一个字节数组
                        byte[] buffer = _sendQueue.Dequeue();
                        if (buffer.Length + ms.Length <= GameEntry.Socket.MaxSendByteCount) {
                            smallCount++;
                            ms.Write(buffer, 0, buffer.Length);
                        } else {
                            //有未处理的字节
                            _unDealBytes = buffer;
                            _isHasUnDealBytes = true;
                            break;//非常重要
                        }
                    }
                    _sendCount++;
                    Debug.Log("拼凑的小包数量:" + smallCount);
                    Send(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 封装数据包
        /// </summary>
        private byte[] MakeData(byte[] data) {
            byte[] retBuffer = null;
            //1.如果数据包的长度 大于了mCompressLen 则进行压缩
            bool isCompress = data.Length > _compressLen;
            if (isCompress) {
                data = ZlibHelper.CompressBytes(data);
            }

            //2.异或
            data = SecurityUtil.Xor(data);

            //3.Crc校验 压缩后的
            ushort crc = Crc16.CalculateCrc16(data);

            SpriteMemoryStream ms = _socketSendMS;
            ms.SetLength(0);

            ms.WriteUShort((ushort)(data.Length + 3));
            ms.WriteBool(isCompress);
            ms.WriteUShort(crc);
            ms.Write(data, 0, data.Length);

            retBuffer = ms.ToArray();
            return retBuffer;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMsg(byte[] buffer) {
            //得到封装后的数据包
            byte[] sendBuffer = MakeData(buffer);

            lock (_sendQueue) {
                //把数据包加入队列
                _sendQueue.Enqueue(sendBuffer);
            }
        }

        /// <summary>
        /// 真正发送数据包到服务器
        /// </summary>

        private void Send(byte[] buffer) {
            _client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, _client);
        }

        /// <summary>
        /// 发送数据包的回调
        /// </summary>
        private void SendCallBack(IAsyncResult ar) {
            _client.EndSend(ar);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        private void ReceiveMsg() {
            _client.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, ReceiveCallBack, _client);
        }

        #region 接收数据回调
        /// <summary>
        /// 接收数据回调
        /// </summary>
        private void ReceiveCallBack(IAsyncResult ar) {
            try {
                int len = _client.EndReceive(ar);
                if (len > 0) {
                    //已经接收到数据
                    //把接收到数据 写入缓冲数据流的尾部
                    _receiveMS.Position = _receiveMS.Length;
                    //把指定长度的字节 写入数据流
                    _receiveMS.Write(_receiveBuffer, 0, len);
                    //如果缓存数据流的长度>2 说明至少有个不完整的包过来了
                    //为什么这里是2 因为我们客户端封装数据包 用的ushort 长度就是2
                    if (_receiveMS.Length > 2) {
                        //进行循环 拆分数据包
                        while (true) {
                            //把数据流指针位置放在0处
                            _receiveMS.Position = 0;

                            //currMsgLen = 包体的长度
                            int currMsgLen = _receiveMS.ReadUShort();

                            //currFullMsgLen 总包的长度=包头长度+包体长度
                            int currFullMsgLen = 2 + currMsgLen;

                            //如果数据流的长度>=整包的长度 说明至少收到了一个完整包
                            if (_receiveMS.Length >= currFullMsgLen) {
                                //至少收到一个完整包
                                //定义包体的byte[]数组
                                byte[] buffer = new byte[currMsgLen];

                                //把数据流指针放到2的位置 也就是包体的位置
                                _receiveMS.Position = 2;

                                //把包体读到byte[]数组
                                _receiveMS.Read(buffer, 0, currMsgLen);

                                lock (_receiveQueue) {
                                    _receiveQueue.Enqueue(buffer);
                                }
                                //==============处理剩余字节数组===================

                                //剩余字节长度
                                int remainLen = (int)_receiveMS.Length - currFullMsgLen;
                                if (remainLen > 0) {
                                    //把指针放在第一个包的尾部
                                    _receiveMS.Position = currFullMsgLen;

                                    //定义剩余字节数组
                                    byte[] remainBuffer = new byte[remainLen];

                                    //把数据流读到剩余字节数组
                                    _receiveMS.Read(remainBuffer, 0, remainLen);

                                    //清空数据流
                                    _receiveMS.Position = 0;
                                    _receiveMS.SetLength(0);

                                    //把剩余字节数组重新写入数据流
                                    _receiveMS.Write(remainBuffer, 0, remainBuffer.Length);

                                    remainBuffer = null;
                                } else {
                                    //没有剩余字节

                                    //清空数据流
                                    _receiveMS.Position = 0;
                                    _receiveMS.SetLength(0);

                                    break;
                                }
                            } else {
                                //还没有收到完整包
                                break;
                            }
                        }
                    }
                    //进行下一次接收数据包
                    ReceiveMsg();
                } else {
                    //客户端断开连接
                    Debug.Log(string.Format("服务器{0}断开连接", _client.RemoteEndPoint.ToString()));
                }
            } catch {
                //客户端断开连接
                Debug.Log(string.Format("服务器{0}断开连接", _client.RemoteEndPoint.ToString()));
            }
        }
        #endregion

    }
}
