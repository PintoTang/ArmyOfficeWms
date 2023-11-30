using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Server
{
    public delegate void MessageReportDelegate(string uid, string message);
    public delegate void NotifyConnectedStatusChangeHandler(string uid, bool isConnected, string message);
    public delegate void NotifyMessageHandler(string uid, byte[] bytes);
    public abstract class SocketAsyncServerAbstract
    {
        public int ListenCount
        {
            get { return _listenCount; }
            set { _listenCount = value; }
        }

        public ProtocolType ProtocolType
        {
            get { return _protocolType; }
            set { _protocolType = value; }
        }

        public string LocalIp
        {
            get { return _localIp; }
            set { _localIp = value; }
        }

        public int LocalPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }

        private Socket _listenSocket;

        /// <summary>
        /// 当前连接数:并发的连接数量(1000以上)
        /// </summary>
        private int _maxConnections = 200;


        private ProtocolType _protocolType = ProtocolType.Tcp;


        /// <summary>
        /// 最大并发量
        /// </summary>
        private Int32 _maxConcurrence = 1000;

        /// <summary>
        /// 接受缓存大小:每一个收发缓冲区的大小(32768)
        /// </summary>
        private Int32 _receiveBufferSize = 32768;

        /// <summary>
        /// 并发控制信号量
        /// </summary>
        private Semaphore _semaphoreAcceptedClients;

        /// <summary>
        /// Socket连接池
        /// </summary>
        private SocketAsyncEventArgsPool _readWritePool;

        private int _listenCount = 10;
        private string _localIp="127.0.0.1";
        private int _localPort=9001;

        public int DeviceId { get; set; }

        /// <summary>
        /// 最大并发量
        /// </summary>
        public int MaxConcurrence
        {
            get { return _maxConcurrence; }
            set { _maxConcurrence = value; }
        }

        /// <summary>
        /// 当前连接数:并发的连接数量(1000以上)
        /// </summary>
        public int MaxConnections
        {
            get { return _maxConnections; }
            set { _maxConnections = value; }
        }

        /// <summary>
        /// 接受缓存大小:每一个收发缓冲区的大小(32768)
        /// </summary>
        public int ReceiveBufferSize
        {
            get { return _receiveBufferSize; }
            set { _receiveBufferSize = value; }
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult InitilizeConfig();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public OperateResult Initilize(int deviceId)
        {
            DeviceId = deviceId;
            OperateResult initConfigResult = InitilizeConfig();
            if (!initConfigResult.IsSuccess)
            {
                return initConfigResult;
            }
            OperateResult listenResult = InitilizeListen();
            if (!listenResult.IsSuccess)
            {
                return listenResult;
            }
            return OperateResult.CreateSuccessResult("成功");
        }

        private OperateResult InitilizeListen()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                _semaphoreAcceptedClients = new Semaphore(MaxConcurrence, MaxConcurrence);
                _readWritePool = new SocketAsyncEventArgsPool(MaxConcurrence);

                SocketAsyncEventArgsWithId readWriteEventArgWithId;
                //构造最大并发数量的SocketAsyncEventArgsWithId对象，并同时开辟接受和发送SocketAsyncEventArgs，放回到读写池
                for (Int32 i = 0; i < MaxConcurrence; i++)
                {
                    readWriteEventArgWithId = new SocketAsyncEventArgsWithId();
                    readWriteEventArgWithId.ReceiveSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                    byte[] buffer = new byte[_receiveBufferSize];
                    readWriteEventArgWithId.ReceiveSAEA.SetBuffer(buffer, 0, buffer.Length);
                    _readWritePool.Push(readWriteEventArgWithId);
                }

                IPAddress localIPAddress = string.IsNullOrEmpty(LocalIp) ? IPAddress.Any : IPAddress.Parse(LocalIp);
                IPEndPoint localEndPoint = new IPEndPoint(localIPAddress, LocalPort);
                _listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType);
                if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    _listenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                    _listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
                }
                else
                {
                    _listenSocket.Bind(localEndPoint);
                }
                //挂起的连接数最大3
                _listenSocket.Listen(ListenCount);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertExMessage(ex);

            }
            return result;
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }


        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.LastOperation != SocketAsyncOperation.Receive)
            {
                return;
            }

            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (NotifyMessageHandlerEvent != null)
                    {
                        Int32 byteTransferred = e.BytesTransferred;
                        byte[] pubBuffer = new byte[byteTransferred];
                        int iOffset = e.Offset;
                        Buffer.BlockCopy(e.Buffer, iOffset, pubBuffer, 0, byteTransferred);

                        MySocketAsyncEventArgs msaea = (MySocketAsyncEventArgs)e;

                        NotifyMessageHandlerEvent.BeginInvoke(msaea.Uid, pubBuffer, null, null);
                    }
                }
                //可以在这里设一个停顿来实现间隔时间段监听，这里的停顿是单个用户间的监听间隔
                //发送一个异步接受请求，并获取请求是否为成功
                Socket socket = e.UserToken as Socket;
                Boolean willRaiseEvent = socket != null && socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    Task.Run(() => { ProcessReceive(e); });
                }
            }
            else//客户端主动断开连接
            {
                ReportConnectedStatusChange(((MySocketAsyncEventArgs)e).Uid, false, "接收到消息异常，断开客户端->" + ((MySocketAsyncEventArgs)e).Uid);
                CloseClientSocket(((MySocketAsyncEventArgs)e).Uid);
            }
        }

        private void ReportConnectedStatusChange(string uid, bool isConnected, string message)
        {
            if (NotifyConnectedStatusChangeEvent != null)
            {
                NotifyConnectedStatusChangeEvent(uid, isConnected, message);
            }
        }

        private void CloseClientSocket(string uid)
        {
            if (String.IsNullOrEmpty(uid) || uid == "-1" || _readWritePool == null)
                return;
            SocketAsyncEventArgsWithId saeaw = _readWritePool.FindByUid(uid);
            if (saeaw == null)
                return;
            MySocketAsyncEventArgs maea = saeaw.ReceiveSAEA;
            Socket socket = maea.UserToken as Socket;
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Disconnect(false);
            }
            catch (Exception ex)
            {
                String strErrorMsg = String.Format("CloseClientSocketException {0}-{3}：错误消息：{1}，栈消息：{2}\n", DateTime.Now.ToString(), ex.Message, ex.StackTrace, uid);
                ReportMessage(uid, strErrorMsg);
            }

            _semaphoreAcceptedClients.Release();
            _readWritePool.Push(saeaw);
            Interlocked.Decrement(ref _maxConnections);
        }

        private void ReportMessage(string uid, string message)
        {
            if (MessageReportEvent != null)
            {
                MessageReportEvent(uid, message);
            }
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="sendBytes"></param>
        public bool Send(string uid, byte[] sendBytes)
        {
            if (String.IsNullOrEmpty(uid))
            {
                return false;
            }
            SocketAsyncEventArgsWithId socketWithId = _readWritePool.FindByUid(uid);
            if (socketWithId == null)
            {
                return false;
            }

            MySocketAsyncEventArgs e = socketWithId.SendSAEA;
            Socket socket = e.UserToken as Socket;
            bool result = false;
            if (e.SocketError == SocketError.Success)
            {
                try
                {
                    if (socket != null)
                    {
                        result = socket.Send(sendBytes) > 0;
                    }
                    else
                    {
                        ReportMessage(uid, "发送恢复消息失败：Socket对象为空");
                    }
                }
                catch (Exception ex)
                {
                    ReportMessage(uid, "发送恢复消息失败：" + ex.Message);
                }
            }
            else
            {
                this.CloseClientSocket(e.Uid);
            }
            return result;
        }

        /// <summary>
        /// 异常消息上报事件
        /// </summary>
        public event MessageReportDelegate MessageReportEvent;

        /// <summary>
        /// 客户端建立、断开连接消息上报
        /// </summary>
        public event NotifyConnectedStatusChangeHandler NotifyConnectedStatusChangeEvent;

        /// <summary>
        /// 收到消息上报
        /// </summary>
        public event NotifyMessageHandler NotifyMessageHandlerEvent;
    }
}
