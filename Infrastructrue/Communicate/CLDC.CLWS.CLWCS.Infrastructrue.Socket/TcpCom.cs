using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets
{
    /// <summary>
    /// Tcp通讯类 包含客户端和服务端
    /// </summary>
    public class TcpCom : ComBase
    {

        /// <summary>
        /// Tcpsocket起始socket
        /// </summary>
        Socket _tcpSocket = null;
        /// <summary>
        /// _RvSocket收取端
        /// </summary>
        Socket _receveSocket = null;
        List<Socket> _connSocketList = null;//TcpCom作为服务端时连接客户端集合
        Dictionary<int, Timer> _polTimers = null; //连接探测器集合
        /// <summary>
        /// TCP服务端标志
        /// </summary>
        bool _isSever = false;
        /// <summary>
        /// 本地IP
        /// </summary>
        private IPAddress _localIp;
        /// <summary>
        /// 本地IP端口
        /// </summary>
        private int _localIpPort;


        /// <summary>
        /// 远端IP
        /// </summary>
        private IPAddress _remoteIp;
        /// <summary>
        /// 远端IP端口
        /// </summary>
        private int _remoteIpPort;
        /// <summary>
        /// 监听线程
        /// </summary>
        private Thread _thrdListen;
        private float _timeSpac = 1; //连接定时器时间秒
        private bool _connected;

        private object sendlock;//发送锁
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="LocalIp">本地IP</param>
        /// <param name="LocalIpPort">本地IP端口</param>
        public TcpCom(IPAddress LocalIp, int LocalIpPort)
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _localIp = LocalIp;
            _localIpPort = LocalIpPort;
            sendlock = new object();
            _IsLinkProbeMark = false;
            _connected = false;
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        public TcpCom()
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sendlock = new object();
            _IsLinkProbeMark = false;
            _connected = false;
        }
        /// <summary>
        /// 析构方法
        /// </summary>
        ~TcpCom()
        {
            _tcpSocket.Close();
            if (_receveSocket != null) _receveSocket.Close();
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bysts">发送的字节数组</param>
        /// <returns></returns>
        public override int Send(byte[] bysts)
        {
            lock (sendlock)
            {
                int Snlen = 0;
                try
                {
                    if (_isSever)
                    {
                        SelectSocketClient();
                        Snlen = _receveSocket.Send(bysts);
                        Console.WriteLine(_receveSocket.RemoteEndPoint.ToString());
                    }
                    else
                    {
                        Snlen = _tcpSocket.Send(bysts);
                    }
                }
                catch (Exception ex)
                {
                    string strMessage = ex.Message.ToString();
                }
                return Snlen;
            }
        }
        /// <summary>
        ///  发送数据
        /// </summary>
        /// <param name="bysts">发送字节数组</param>
        /// <param name="indexskt">发送端序号</param>
        /// <returns></returns>
        public int Send(byte[] bysts, int indexskt)
        {
            lock (sendlock)
            {
                int Snlen = 0;

                try
                {
                    if (_isSever)
                    {
                        SelectSocketClient(indexskt);
                        Snlen = _receveSocket.Send(bysts);
                    }
                    else
                    {
                        Snlen = _tcpSocket.Send(bysts);
                    }
                }
                catch (Exception ex)
                {
                    string strMessage = ex.Message.ToString();
                }
                return Snlen;
            }
        }
        /// <summary>
        /// TcpCom组件作为服务端时，选择所连接客户端
        /// </summary>
        private void SelectSocketClient()
        {
            foreach (Socket skt in _connSocketList)
            {
                IPEndPoint Ipendpnt = skt.RemoteEndPoint as IPEndPoint;
                if (Ipendpnt.Address.ToString() == this._remoteIp.ToString()
                    && Ipendpnt.Port == this._remoteIpPort)
                {
                    _receveSocket = skt;
                    break;
                }
            }
        }
        /// <summary>
        ///  TcpCom组件作为服务端时，选择所连接客户端
        /// </summary>
        /// <param name="indexskt">客户端索引</param>
        private void SelectSocketClient(int indexskt)
        {
            _receveSocket = _connSocketList[indexskt];
            IPEndPoint Ipendpnt = _receveSocket.RemoteEndPoint as IPEndPoint;
            _remoteIp = Ipendpnt.Address;
            _remoteIpPort = Ipendpnt.Port;
        }
        /// <summary>
        /// 收取数据
        /// </summary>
        public override int Receive(byte[] bytes)
        {
            bytes = ReciveBytes;
            int n = 0, bs = 0;
            for (int i = 0; i < ReciveBytes.Length; i++)
            {
                if (ReciveBytes[i] == 0)
                {
                    n++;
                }
                if (n == 10)
                {
                    bs = i;
                    break;
                }
            }

            return ReciveBytes.Length;

        }
        /// <summary>
        /// 定时器连接时间间隔秒
        /// </summary>
        public float TimeSpac
        {
            get { return _timeSpac; }
            set { _timeSpac = value; }
        }
        /// <summary>
        /// 远端连接IP
        /// </summary>
        public IPAddress RemoteIp
        {
            get
            {
                return _remoteIp;
            }
            set
            {
                _remoteIp = value;
            }
        }

        /// <summary>
        /// 远端连接IP端口
        /// </summary>
        public int RemoteIpPort
        {
            get
            {
                return _remoteIpPort;
            }
            set
            {
                _remoteIpPort = value;
            }

        }
        /// <summary>
        /// 本地IP地址
        /// </summary>
        public IPAddress LocalIp
        {
            get { return _localIp; }
            set { _localIp = value; }
        }
        /// <summary>
        /// 本地IP端口
        /// </summary>
        public int LocalIpPort
        {
            get { return _localIpPort; }
            set { _localIpPort = value; }
        }
        /// <summary>
        /// TCP服务端标志
        /// </summary>
        public bool IsSever
        {
            get
            {
                return _isSever;
            }
            set
            {
                _isSever = value;
            }
        }
        private bool _IsLinkProbeMark;
        /// <summary>
        /// 是否启动链接探测器:true启动；flase：停止
        /// </summary>
        public bool IsLinkProbeMark
        {
            get { return _IsLinkProbeMark; }
            set { _IsLinkProbeMark = value; }
        }
        /// <summary>
        /// TCP作为客户端建立连接
        /// </summary>
        public void Connect()
        {
            try
            {
                if (_isSever)
                {
                    IPEndPoint LocalIpendPoint = new IPEndPoint(_localIp, _localIpPort);
                    _tcpSocket.Bind(LocalIpendPoint);
                    _connSocketList = new List<Socket>();//实例SocketTcp服务端的连接客户端集合
                }
                else
                {
                    IPEndPoint RemoteIpendpoint = new IPEndPoint(_remoteIp, _remoteIpPort);
                    _tcpSocket.Connect(RemoteIpendpoint);
                    IPEndPoint ipdp = _tcpSocket.LocalEndPoint as IPEndPoint;
                    _localIp = ipdp.Address;
                    _localIpPort = ipdp.Port;
                }
                _polTimers = new Dictionary<int, Timer>();//实例连接探测器集合
                StartWork();
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
            }

        }
        /// <summary>
        /// 获取连接标志
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
            //set { _connected = value; }
        }
        /// <summary>
        /// 开始工作
        /// </summary>
        private void StartWork()
        {
            if (_isSever)
            {
                _thrdListen = new Thread(new ThreadStart(Listen));
                _thrdListen.Name = "TcpCom Server Listen";
                _thrdListen.Start();

            }
            else
            {
                Thread ThrdrvData = new Thread(new ParameterizedThreadStart(ReceiveBuffer));
                ThrdrvData.Name = "TcpCom Cleint RvData";
                ThrdrvData.Start(_tcpSocket);
            }
        }

        /// <summary>
        /// Tcp作为服务端监听
        /// </summary>
        private void Listen()
        {
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    if (_tcpSocket.Connected) continue;
                    _tcpSocket.Listen(32);
                    Socket srvsocket = _tcpSocket.Accept();
                    _connSocketList.Add(srvsocket);
                    _receveSocket = srvsocket;
                    IPEndPoint LongIpendpoint = (IPEndPoint)srvsocket.RemoteEndPoint;
                    _remoteIp = LongIpendpoint.Address;
                    _remoteIpPort = LongIpendpoint.Port;
                    Thread ThrdrvData = new Thread(new ParameterizedThreadStart(ReceiveBuffer));
                    ThrdrvData.Name = "TcpCom Server RvData No: " + _connSocketList.Count.ToString();
                    ThrdrvData.Start(srvsocket);
                }
                catch (Exception ex)
                {

                }
            }

        }
        /// <summary>
        /// 收取数据
        /// </summary>
        /// <param name="connecter">连接socket</param>
        private void ReceiveBuffer(object connecter)
        {
            Socket cnntsocket = connecter as Socket;
            int timespac = Convert.ToInt32(1000 * _timeSpac);
            Timer PolTimer = new Timer(new TimerCallback(LinkProbe), cnntsocket, 0, timespac);  //生成连接探测器
            _polTimers.Add(Thread.CurrentThread.ManagedThreadId, PolTimer);
            if (IsSever)
            {

                TcpServerReceiveData(cnntsocket);
            }
            else
            {
                TcpClientReceiveData(cnntsocket);
            }
        }
        /// <summary>
        /// 作为客户端模式收取数据
        /// </summary>
        private void TcpClientReceiveData(Socket conectsocket)
        {
            while (conectsocket.Connected)
            {
                _connected = conectsocket.Connected;
                if (conectsocket.Available > 0)
                {
                    byte[] rvbytes = new byte[256];
                    int rvlen = conectsocket.Receive(rvbytes);
                    base.ReciveBytes = new byte[rvlen];
                    Array.Copy(rvbytes, base.ReciveBytes, rvlen);
                    base.ReceiveData_Event(base.ReciveBytes);
                    Console.WriteLine(conectsocket.LocalEndPoint);
                }

                Thread.Sleep(100);
                
            }
            _connected = conectsocket.Connected;
            conectsocket.Close(); //_TcpSocket与服务端断开后必须关闭释放连接的_TcpSocket
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //重新生成
            _polTimers[Thread.CurrentThread.ManagedThreadId].Dispose(); // 释放连接探测器
            _polTimers.Remove(Thread.CurrentThread.ManagedThreadId);    //清除连接探测器
            Console.WriteLine("编号:" + Thread.CurrentThread.ManagedThreadId + "  名称:" + Thread.CurrentThread.Name + "退出");
        }
        /// <summary>
        /// 作为服务端模式收取数据
        /// </summary>
        private void TcpServerReceiveData(Socket conectsocket)
        {
            while (conectsocket.Connected)
            {
                _connected = conectsocket.Connected;
                if (conectsocket.Available > 0)
                {
                    byte[] rvbytes = new byte[256];
                    int rvlen = conectsocket.Receive(rvbytes);
                    base.ReciveBytes = new byte[rvlen];
                    Array.Copy(rvbytes, base.ReciveBytes, rvlen);
                    base.ReceiveData_Event(base.ReciveBytes);
                }
                Thread.Sleep(100);
            }
            _connected = conectsocket.Connected;
            _connSocketList.Remove(conectsocket); //如果连接断开 在客户端集合中移除该客户端
            _polTimers[Thread.CurrentThread.ManagedThreadId].Dispose(); // 释放连接探测器
            _polTimers.Remove(Thread.CurrentThread.ManagedThreadId);    //清除连接探测器
            SelectRvSocket();
            Console.WriteLine("编号:" + Thread.CurrentThread.ManagedThreadId + "  名称:" + Thread.CurrentThread.Name + "退出");

        }
        /// <summary>
        /// 连接探测器
        /// </summary>
        private void LinkProbe(object pol)
        {
            byte[] ProbeByts = new byte[1];
            if (!_IsLinkProbeMark) return;
            try
            {
                SocketCommunicate clskt = pol as SocketCommunicate;
                clskt.Send(ProbeByts);
            }
            catch (Exception ex)
            {
                string strMessage = ex.Message.ToString();
                Console.WriteLine(strMessage);
            }
        }
        private void SelectRvSocket()
        {
            if (_connSocketList.Count == 0) return;
            _receveSocket = _connSocketList[_connSocketList.Count - 1];
            IPEndPoint Ipedpnt = _receveSocket.RemoteEndPoint as IPEndPoint;
            _remoteIp = Ipedpnt.Address;
            _remoteIpPort = Ipedpnt.Port;
        }

        public override bool Close()
        {
            _tcpSocket.Close();
            if (_receveSocket != null) _receveSocket.Close();
            return true;
        }
    }
}
