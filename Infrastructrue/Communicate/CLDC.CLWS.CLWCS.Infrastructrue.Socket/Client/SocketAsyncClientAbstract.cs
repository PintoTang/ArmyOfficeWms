using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client
{
   

    public delegate void ReceivedMessageNotifyDelegate(byte[] datas);

    /// <summary>
    /// 通知状态改变委托
    /// </summary>
    /// <param name="isNormal">状态：True--由坏变好、False--由好变坏</param>
    public delegate void NotifyConnectedStatusChangeHandler(bool isNormal);
    public abstract class SocketAsyncClientAbstract
    {
        private Socket _socket = null;
        private int _remotePoint;
        private int _localPoint;
        private string _remoteIp;
        private string _localIp;
        protected IPEndPoint RemoteIpAndPort = null;
        protected IPEndPoint LocalIpAndPort = null;
        private ProtocolType _protocolType = ProtocolType.Tcp;
        private SocketAsyncEventArgs _socketAsyncEventArgs = null;
        private readonly Semaphore _autoConnectSemaphore = new Semaphore(1, 1);
        private bool _lastConnected = false;



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

        public string RemoteIp
        {
            get { return _remoteIp; }
            set { _remoteIp = value; }
        }

        public int LocalPort
        {
            get { return _localPoint; }
            set { _localPoint = value; }
        }

        public int RemotePort
        {
            get { return _remotePoint; }
            set { _remotePoint = value; }
        }

        public int DeviceId { get; set; }

        public bool IsConnected
        {
            get
            {
                if (_socket == null)
                {
                    return false;
                }
                return _socket.Connected;
            }
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

            StartConnect();

            return OperateResult.CreateSuccessResult("成功");
        }

        /// <summary>
        /// 启动连接
        /// </summary>
        private void StartConnect()
        {
            Thread thread = new Thread(ConnectServer);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// KeepAlive参数设置
        /// </summary>
        /// <param name="onOff">是否启用Keep-Alive</param>
        /// <param name="keepAliveTime">多长时间后开始第一次探测（单位：毫秒）</param>
        /// <param name="keepAliveInterval">探测时间间隔（单位：毫秒）</param>
        /// <returns></returns>
        private byte[] KeepAlive(int onOff, int keepAliveTime, int keepAliveInterval)
        {
            byte[] buffer = new byte[12];
            BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
            BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
            BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
            return buffer;
        }
        private void ConnectServer()
        {
            while (true)
            {
                _autoConnectSemaphore.WaitOne();
                try
                {
                    if (_socket != null)
                    {
                        _socket.Close();
                    }

                    if (_protocolType == ProtocolType.Tcp)
                    {
                        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, _protocolType);
                        _socket.SendBufferSize = 0;
                    }
                    else if (_protocolType == ProtocolType.Udp)
                    {
                        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, _protocolType);
                        _socket.DontFragment = false;
                        _socket.MulticastLoopback = false;
                        _socket.EnableBroadcast = false;
                    }
                    else
                    {
                        ReportMessage("SocketClient暂时只支持UDP和TCP!", EnumLogLevel.Error);
                        break;
                    }

                    _socket.Bind(LocalIpAndPort);

                    //将变量全局，便于资源释放
                    _socketAsyncEventArgs = new SocketAsyncEventArgs();
                    _socketAsyncEventArgs.Completed += SocketAsyncReceivedCompleted;

                    _socket.IOControl(IOControlCode.KeepAliveValues, KeepAlive(1, 1000, 1000), null);
                    _socket.Connect(RemoteIpAndPort);
                    _socketAsyncEventArgs.UserToken = _socket;
                    byte[] buffer = new byte[1024];
                    _socketAsyncEventArgs.SetBuffer(buffer, 0, buffer.Length);
                    ReportMessage("连接服务端成功", EnumLogLevel.Info);
                    bool willRaiseEvent = _socket.ReceiveAsync(_socketAsyncEventArgs);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(_socketAsyncEventArgs);
                    }
                    if (NotifyConnectedStatusChangeEvent != null)
                    {
                        NotifyConnectedStatusChangeEvent(true);
                    }
                    _lastConnected = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(2000);
                    DisconnectService();
                    ReportMessage("Socket连接异常,详细信息为：" + ex.Message, EnumLogLevel.Error);
                }
            }
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>是否发送成功</returns>
        public OperateResult Send(byte[] message)
        {
            OperateResult sendResult = OperateResult.CreateFailedResult();
            try
            {
                if (_socket != null && _socket.Connected)
                {
                    int lenght = _socket.Send(message);
                    sendResult.Message = string.Format("发送数据：{0}", lenght);
                    sendResult.IsSuccess = true;
                    return sendResult;
                }
                sendResult.IsSuccess = false;
                sendResult.Message = "发送数据失败";
            }
            catch (Exception ex)
            {
                DisconnectService();
                ReportMessage("Socket发送消息异常，详细信息" + ex.Message, EnumLogLevel.Error);
                sendResult.IsSuccess = false;
                sendResult.Message = "Socket发送消息异常，详细信息" + ex.Message;
            }

            return sendResult;
        }

        /// <summary>
        /// 处理接受的消息
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.LastOperation != SocketAsyncOperation.Receive
                || (e.LastOperation == SocketAsyncOperation.Receive && e.BytesTransferred < 1))
            {
                DisconnectService();
                return;
            }

            if (e.SocketError == SocketError.Success)
            {
                if (ReceivedMessageNotifyEvent != null)
                {
                    Int32 byteTransferred = e.BytesTransferred;
                    byte[] pubBuffer = new byte[byteTransferred];
                    int iOffset = e.Offset;
                    Buffer.BlockCopy(e.Buffer, iOffset, pubBuffer, 0, byteTransferred);
                    ReceivedMessageNotifyEvent(pubBuffer);
                }
            }
            Socket socket = e.UserToken as Socket;
            if (socket == null || !socket.Connected)
            {
                DisconnectService();
                return;
            }
            Task.Run(() =>
            {
                Boolean willRaiseEvent = socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            });
        }

        private void ReportMessage(string message,EnumLogLevel messageLeve)
        {
            if (MessageReportEvent != null)
            {
                MessageReportEvent(message,messageLeve);
            }
        }
        private void ReleaseConnectionSemaphore()
        {
            try
            {
                _autoConnectSemaphore.Release();
            }
            catch (Exception ex)
            {
                ReportMessage(string.Format("释放信号量失败，异常信息如下：{0}", ex.Message),EnumLogLevel.Error);
            }
        }

        private void SocketAsyncReceivedCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }
        public void DisconnectService()
        {
            if (_socket != null)
            {
                try
                {
                    //关闭服务的时候，注销完成事件
                    if (_socketAsyncEventArgs != null)
                    {
                        _socketAsyncEventArgs.Completed -= SocketAsyncReceivedCompleted;
                        _socketAsyncEventArgs.Dispose();
                        _socketAsyncEventArgs = null;
                    }
                    if (_lastConnected)
                    {
                        ReportMessage("通信断开重连......",EnumLogLevel.Error);
                        _socket.Shutdown(SocketShutdown.Both);
                        _socket.Disconnect(false);
                        if (NotifyConnectedStatusChangeEvent != null)
                        {
                            NotifyConnectedStatusChangeEvent(false);
                        }
                    }
                    _socket.Close();
                    _lastConnected = false;

                }
                catch (Exception ex)
                {
                    ReportMessage("Socket断开异常，详细信息" + ex.Message, EnumLogLevel.Error);
                }
                finally
                {
                    ReleaseConnectionSemaphore();
                }
            }
        }

        /// <summary>
        /// 接收到的消息上报事件：用来上报异步接收到的报文
        /// </summary>
        public event ReceivedMessageNotifyDelegate ReceivedMessageNotifyEvent;

        /// <summary>
        /// 消息上报事件:用来上报异常消息等
        /// </summary>
        public event MessageReportDelegate MessageReportEvent;

        /// <summary>
        /// 消息上报事件:用来通信状态改变等
        /// </summary>
        public event NotifyConnectedStatusChangeHandler NotifyConnectedStatusChangeEvent;
    }
}
