using System.Collections.Generic;

namespace SpriteFramework
{
    /// <summary>
    /// Socket 管理器
    /// </summary>
    public class SocketManager
    {

        /// <summary>
        /// 每帧最大发送数量
        /// </summary>
        public int MaxSendCount = 5;
        /// <summary>
        /// 每次发包最大的字节
        /// </summary>
        public int MaxSendByteCount = 1024;

        /// <summary>
        /// 每帧最大处理包数量
        /// </summary>
        public int MaxReceiveCount = 5;

        /// <summary>
        /// 发送用的MemoryStream
        /// </summary>
        public SpriteMemoryStream SocketSendMS { get; private set; }
        /// <summary>
        /// 接收用的MemoryStream
        /// </summary>
        public SpriteMemoryStream SocketReceiveMS { get; private set; }

        /// <summary>
        /// 主Socket
        /// </summary>
        private SocketTcpRoutine m_MainSocketRoutine;

        /// <summary>
        /// SocketTcp访问器的链表
        /// </summary>
        private LinkedList<SocketTcpRoutine> _socketTcpRoutineList;

        public SocketManager() {
            _socketTcpRoutineList = new LinkedList<SocketTcpRoutine>();

            SocketSendMS = new SpriteMemoryStream();
            SocketReceiveMS = new SpriteMemoryStream();

            m_MainSocketRoutine = CreateSocketTcpRoutine();
            SocketProtoListener.AddProtoListener();
        }

        /// <summary>
        /// 连接主Socket
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        public void ConnectToMainSocket(string ip, int port) {
            m_MainSocketRoutine.Connect(ip, port);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMsg(byte[] buffer) {
            m_MainSocketRoutine.SendMsg(buffer);
        }

        /// <summary>
        /// 创建SocketTcp访问器
        /// </summary>
        public SocketTcpRoutine CreateSocketTcpRoutine() {
            //从池中获取
            return GameEntry.Pool.ClassObjectPool.Dequeue<SocketTcpRoutine>();
        }

        /// <summary>
        /// 注册SocketTcp访问器
        /// </summary>
        internal void RegisterSocketTcpRoutine(SocketTcpRoutine routine) {
            _socketTcpRoutineList.AddFirst(routine);
        }

        /// <summary>
        /// 移除SocketTcp访问器
        /// </summary>
        internal void RemoveSocketTcpRoutine(SocketTcpRoutine routine) {
            _socketTcpRoutineList.Remove(routine);
        }

        internal void OnUpdate() {
            for (LinkedListNode<SocketTcpRoutine> curRoutine = _socketTcpRoutineList.First; curRoutine != null; curRoutine = curRoutine.Next) {
                curRoutine.Value.OnUpdate();
            }
        }

        public void Dispose() {
            _socketTcpRoutineList.Clear();

            m_MainSocketRoutine.DisConnect();
            GameEntry.Pool.ClassObjectPool.Enqueue(m_MainSocketRoutine);
            SocketProtoListener.RemoveProtoListener();

            SocketSendMS.Dispose();
            SocketReceiveMS.Dispose();

            SocketSendMS.Close();
            SocketReceiveMS.Close();
        }
    }
}
